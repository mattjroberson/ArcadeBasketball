using UnityEngine;

public class ActionsScript : MonoBehaviour
{
    private PlayerScript player;
    private PlayerStateScript playerStates;
    public ActionEvents events;

    private JumpAction jumpAction;
    private SprintAction sprintAction;
    private ShootAction shootAction;
    private DunkAction dunkAction;
    private StealAction swipeAction;

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
        swipeAction = new StealAction(player);

        events.onWalkViolation += shootAction.PlayerWalked;
        events.onEnduranceDepleted += sprintAction.Stop;

        GameEvents.Instance.onPossessionChange += PossessionChangeEvent;
        GameEvents.Instance.onPassSent += PassSentEvent;
        GameEvents.Instance.onPassReceived += PassReceivedEvent;
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
        BallEvents.Instance.BallShot(player.CurrentGoal, madeShot);

        playerStates.HasBall = false;
    }

    public void StartPassing()
    {
        PlayerScript target = player.Teammate;

        events.PassBegin(target.transform);
        BallEvents.Instance.BallPassed(target);
    }

    public void TouchBall()
    {
        if (playerStates.HasBall) return;
        BallEvents.Instance.BallTouched(player);
    }

    public void ReachForSteal()
    {
        events.SwipeBegin();
    }

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

    public SprintAction GetSprintAction() { return sprintAction; }

    public ShootAction GetShootAction() { return shootAction;  }

    public DunkAction GetDunkAction() { return dunkAction; }

    public StealAction GetSwipeAction() { return swipeAction; }
}
