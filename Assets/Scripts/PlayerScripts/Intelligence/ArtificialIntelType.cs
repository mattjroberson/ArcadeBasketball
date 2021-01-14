using UnityEngine;

public abstract class ArtificialIntelType : IntelligenceType
{
    protected enum IntelState { MOVING, IDLE }
    protected IntelState state = IntelState.IDLE;

    private const float TARGET_MARGIN = .05f;
    protected Vector2 target;

    protected bool trackingLooseBall = false;

    public ArtificialIntelType(PlayerScript player, ActionsScript actions) : base(player, actions) { }

    public override void UpdateIntelligence()
    {
        if (trackingLooseBall) {
            CheckForJump();
        }

        if (state == IntelState.MOVING) {
            Vector2 dir = (target - player.States.FeetPosition).normalized;
            player.States.CurrentMoveDirection = dir;

            if (Vector2.Distance(player.States.FeetPosition, target) < TARGET_MARGIN) {
                StopMoving();
            }
        }
    }

    public override void Wake()
    {
        base.Wake();
        GameEvents.Instance.onBallLoose += BallLooseEvent;
        GameEvents.Instance.onPossessionChange += PossessionChangedEvent;
    }

    public override void Sleep()
    {
        base.Sleep();
        GameEvents.Instance.onBallLoose -= BallLooseEvent;
        GameEvents.Instance.onPossessionChange -= PossessionChangedEvent;
        StopMoving();
    }

    protected void MoveToSpot(Vector2 newTarget)
    {
        target = newTarget;
        state = IntelState.MOVING;
    }

    private void StopMoving()
    {
        player.States.CurrentMoveDirection = Vector2.zero;
        state = IntelState.IDLE;
    }

    private void BallLooseEvent(Vector2 ballFloor)
    {
        float playerDist = Vector2.Distance(ballFloor, player.States.FloorPosition);
        float teammateDist = Vector2.Distance(ballFloor, player.Teammate.States.FloorPosition);

        if (playerDist < teammateDist) {
            trackingLooseBall = true;
            MoveToSpot(ballFloor);
        }
    }

    private void PossessionChangedEvent(PlayerScript player)
    {
        trackingLooseBall = false;
    }

    private void CheckForJump()
    {
        float xDist = Mathf.Abs(player.transform.position.x - GameLogicScript.Instance.BallTransform.position.x);
        float yDist = GameLogicScript.Instance.BallTransform.position.y - player.States.FloorPosition.y;

        if (xDist < .55f && (yDist < 2.35f) && (yDist > 1.8f)) actions.GetJumpAction().Start(1f);
    }
}
