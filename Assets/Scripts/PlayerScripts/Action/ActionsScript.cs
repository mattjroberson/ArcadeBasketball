using UnityEngine;

public class ActionsScript : MonoBehaviour
{
    private PlayerScript player;
    public ActionEvents events;

    //TODO Convert to super class instead of interface???
    private IAction jumpAction;
    private IAction sprintAction;
    private ShootAction shootAction;
    private IAction dunkAction;

    public void Awake()
    {
        events = new ActionEvents();
    }

    public void Start()
    {
        player = GetComponent<PlayerScript>();

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
        //Do nothing if the defender fouled
        if (player.CheckForStealFoul() == true) return;

        //If probability calculated for a steal, steal ball
        if (Random.value <= player.GetAttributes().GetStealProbability())
        {
            GameEvents.events.BallStolen(player);
        }
    }

    public void PickupLooseBall(PlayerScript player)
    {
        GameEvents.events.LooseBallPickup(player);
    }

    public void CompleteShotProcess()
    {
        bool madeShot = player.CalculateMadeShot();
        GameEvents.events.BallShot(player.GetGoal(), madeShot);
    }

    public IAction GetJumpAction() { return jumpAction; }

    public IAction GetSprintAction() { return sprintAction; }

    public IAction GetShootAction() { return shootAction;  }

    public IAction GetDunkAction() { return dunkAction; }
}
