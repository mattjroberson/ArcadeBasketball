using UnityEngine;

public class ActionsScript : MonoBehaviour
{
    private PlayerScript player;
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

        jumpAction = new JumpAction(this, player.States);
        sprintAction = new SprintAction(this);
        shootAction = new ShootAction(this, player);
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
        if (player.States.InDunkRange) dunkAction.Start();
        else shootAction.Start();
    }

    public void CompleteJumpShotProcess()
    {
        bool madeShot = CompleteShotProcess();
        BallEvents.Instance.BallShot(player.Team.Side.Goal, madeShot);
    }

    public void CompleteDunkShotProcess()
    {
        bool madeShot = CompleteShotProcess();
        BallEvents.Instance.DunkAttempt(player.Team.Side.Goal, madeShot);
    }

    private bool CompleteShotProcess()
    {
        float probability = player.Attributes.GetShotPercentage(player.States.ShotZoneName);
        player.States.HasBall = false;
        return GameLogicScript.CalculateIfMadeShot(probability);
    }

    public void StartPassing()
    {
        PlayerScript target = player.Teammate;

        events.PassBegin(target.transform);
        BallEvents.Instance.BallPassed(target);
    }

    public void HandTouchBall()
    {
        if (player.States.HasBall) return;
        BallEvents.Instance.BallTouchedHand(player);
    }

    public void FootTouchBall()
    {
        if (player.States.HasBall) return;
        BallEvents.Instance.BallTouchedFoot(player);
    }

    public void TouchAiZone(string zone)
    {
        events.TouchedAiZone(zone);
    }

    public void SetShotZoneFlag(Collider2D other, bool val)
    {
        if (other.name == "auto_dunk") {
            player.States.InAutoDunkZone = val;
        }
        else if (other.name == "moving_dunk") {
            player.States.InMovingDunkZone = val;
        }
        else if (other.tag == "shotZone") {
            if (val == true) player.States.ShotZoneName = other.name;
        }
    }

    public void ReachForSteal()
    {
        events.SwipeBegin();
    }

    public void SwitchPlayer()
    {
        GameEvents.Instance.UserPlayerSwitch(player);
    }

    private void PossessionChangeEvent(PlayerScript newBallHandler)
    {
        player.States.HasBall = (newBallHandler == player);
    }

    private void PassSentEvent()
    {
        player.States.IsFrozen = true;
    }

    private void PassReceivedEvent()
    {
        player.States.IsFrozen = false;
    }

    public void EnduranceDepleted() { events.EnduranceDepleted(); }

    public void WalkViolation() { events.WalkViolation(); }

    public JumpAction GetJumpAction() { return jumpAction; }

    public SprintAction GetSprintAction() { return sprintAction; }

    public ShootAction GetShootAction() { return shootAction;  }

    public DunkAction GetDunkAction() { return dunkAction; }

    public StealAction GetSwipeAction() { return swipeAction; }
}
