using System.Collections;
using UnityEngine;

public class ActionsScript : MonoBehaviour
{
    private PlayerScript player;
    private PlayerStateScript playerStates;
    public ActionEvents events;

    //TODO Convert to super class instead of interface???
    private JumpAction jumpAction;
    private IAction sprintAction;
    private ShootAction shootAction;
    private IAction dunkAction;

    private int stealAttempts;

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

        GameEvents.events.onPossessionChange += PossessionChangeEvent;
        GameEvents.events.onPassSent += PassSentEvent;
        GameEvents.events.onPassReceived += PassReceivedEvent;
    }

    public void InitializeShot()
    {
        events.ShotInit();

        if (playerStates.InDunkRange) dunkAction.Start();
        else shootAction.Start();
    }

    public void CompleteShotProcess()
    {
        float probability = player.Attributes.GetShotPercentage(playerStates.ShotZoneName);
        bool madeShot = GameLogicScript.CalculateIfMadeShot(probability);
        BallEvents.events.BallShot(player.CurrentGoal, madeShot);

        playerStates.HasBall = false;
    }

    public void StartPassing()
    {
        PlayerScript target = player.Teammate;

        events.PassBegin(target.transform);
        BallEvents.events.BallPassed(target);
    }

    public void TouchBall()
    {
        if (playerStates.HasBall) return;
        BallEvents.events.BallTouched(player);
    }

    #region StealLogic
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
        if (Random.value <= player.Attributes.GetStealProbability())
        {
            BallEvents.events.BallStolen(player);
        }
    }

    private void IncrementStealAttempts()
    {
        if (stealAttempts == 0) StartCoroutine(ClearStealAttempts());
        stealAttempts++;
    }

    private IEnumerator ClearStealAttempts()
    {
        yield return new WaitForSeconds(player.Attributes.GetStealAttemptWindow());
        stealAttempts = 0;
    }

    private bool CheckForStealFoul()
    {
        if (stealAttempts > player.Attributes.GetStealAttemptLimit())
        {
            //If the probability results in a foul, return true
            if (Random.value <= player.Attributes.GetStealFoulProbability())
            {
                Debug.Log("Player Fouled");
                return true;
            }
        }

        return false;
    }
    #endregion

    private void PossessionChangeEvent(PlayerScript newBallHandler)
    {
        playerStates.HasBall = (newBallHandler == player);
    }

    private void PassSentEvent(PlayerScript receiver)
    {
        if (player == receiver) playerStates.WaitingOnPass = true;
    }

    private void PassReceivedEvent(PlayerScript receiver)
    {
        if (playerStates.WaitingOnPass) playerStates.WaitingOnPass = false;
    }

    public void EnduranceDepleted() { events.EnduranceDepleted(); }

    public void WalkViolation() { events.WalkViolation(); }

    public JumpAction GetJumpAction() { return jumpAction; }

    public IAction GetSprintAction() { return sprintAction; }

    public IAction GetShootAction() { return shootAction;  }

    public IAction GetDunkAction() { return dunkAction; }
}
