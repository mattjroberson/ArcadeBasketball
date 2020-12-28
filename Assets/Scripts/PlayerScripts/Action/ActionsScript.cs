using System.Collections;
using UnityEngine;

public class ActionsScript : MonoBehaviour
{
    private PlayerScript player;
    private PlayerStateScript playerStates;
    public ActionEvents events;

    //TODO Convert to super class instead of interface???
    private IAction jumpAction;
    private IAction sprintAction;
    private ShootAction shootAction;
    private IAction dunkAction;

    private int stealAttempts;
    private float stealAttemptTimer;

    public void Awake()
    {
        events = new ActionEvents();
    }

    public void Start()
    {
        player = GetComponent<PlayerScript>();
        playerStates = GetComponent<PlayerStateScript>();

        jumpAction = new JumpAction(this);
        sprintAction = new SprintAction(this);
        shootAction = new ShootAction(this);
        dunkAction = new DunkAction(this);

        events.onWalkViolation += shootAction.PlayerWalked;
        events.onEnduranceDepleted += sprintAction.Stop;
    }

    public void InitializeShot()
    {
        events.ShotInit();

        if (playerStates.GetInDunkRange()) dunkAction.Start();
        else shootAction.Start();
    }

    public void StartPassing()
    {
        //Get a reference to the target
        PlayerScript target = player.GetTeammate();
        
        events.PassBegin(target.transform);
        GameEvents.events.BallPassed(target);
    }

    public void ReachForSteal()
    {
        events.SwipeBegin();
    }

    public void AttemptSteal()
    {
        IncrementStealAttempts();

        //Do nothing if the defender fouled
        if (CheckForStealFoul() == true) return;

        //If probability calculated for a steal, steal ball
        if (Random.value <= player.GetAttributes().GetStealProbability())
        {
            GameEvents.events.BallStolen(player);
        }
    }

    private void IncrementStealAttempts()
    {
        if (stealAttempts == 0) StartCoroutine(ClearStealAttempts());
        stealAttempts++;
    }

    private IEnumerator ClearStealAttempts()
    {
        yield return new WaitForSeconds(player.GetAttributes().GetStealAttemptWindow());
        stealAttempts = 0;
    }

    private bool CheckForStealFoul()
    {
        if (stealAttempts > player.GetAttributes().GetStealAttemptLimit())
        {
            //If the probability results in a foul, return true
            if (Random.value <= player.GetAttributes().GetStealFoulProbability())
            {
                Debug.Log("Player Fouled");
                return true;
            }
        }

        return false;
    }

    public void PickupLooseBall(PlayerScript player)
    {
        GameEvents.events.LooseBallPickup(player);
    }

    public void CompleteShotProcess()
    {
        float probability = player.GetAttributes().GetShotPercentage(playerStates.GetShotZoneName());
        bool madeShot = GameLogicScript.CalculateIfMadeShot(probability);
        GameEvents.events.BallShot(player.GetGoal(), madeShot);
    }

    public void EnduranceDepleted() { events.EnduranceDepleted(); }

    public void WalkViolation() { events.WalkViolation(); }

    public IAction GetJumpAction() { return jumpAction; }

    public IAction GetSprintAction() { return sprintAction; }

    public IAction GetShootAction() { return shootAction;  }

    public IAction GetDunkAction() { return dunkAction; }
}
