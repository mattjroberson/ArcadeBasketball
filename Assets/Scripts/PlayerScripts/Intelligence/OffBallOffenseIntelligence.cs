using System;
using UnityEngine;

public class OffBallOffenseIntelligence : ArtificialIntelligence
{
    //TODO see if these should be abstracted high up
    private enum IntelState { MOVING, IDLE }
    private IntelState state = IntelState.IDLE;
    private TargetScript target;
    private const float TARGET_MARGIN = .05f;
    private const float DECISION_WINDOW = 3f;
    private float decisionTimer = 0;

    private System.Random random = new System.Random();

    public OffBallOffenseIntelligence(PlayerScript player, ActionsScript actions) : base(player, actions) {
        player.Team.IntelStates.onBallHandlerPositionUpdated += BallHandlerPositionUpdatedEvent;
    }

    public override void FixedUpdateIntelligence() { }

    public override void UpdateIntelligence()
    {
        base.UpdateIntelligence();

        if (state == IntelState.IDLE) {
            decisionTimer += Time.deltaTime;

            if(decisionTimer > DECISION_WINDOW) {
                decisionTimer = 0;
                MoveToNewSpot();
            }
        }
        else if(state == IntelState.MOVING) {
            Vector2 dir = (target.PlayerPos - (Vector2)player.transform.position).normalized;
            player.States.CurrentMoveDirection = dir;

            if (Vector2.Distance(player.transform.position, target.PlayerPos) < TARGET_MARGIN) {
                StopMoving();
            }
        }
    }

    public override void Wake()
    {
        MoveToNewSpot();
    }

    private void BallHandlerPositionUpdatedEvent()
    {
        if (player.States.HasBall == false && NeedsToMove()) MoveToNewSpot();
    }

    private void MoveToNewSpot()
    {
        target = PickRandomNewTarget();
        state = IntelState.MOVING;
    }

    private void StopMoving()
    {
        player.States.CurrentMoveDirection = Vector2.zero;
        state = IntelState.IDLE;
    }

    private bool NeedsToMove()
    {
        int check = Array.IndexOf(player.Team.IntelStates.PossibleTargets, target);
        return check == -1;
    }

    private TargetScript PickRandomNewTarget()
    {
        int maxVal = player.Team.IntelStates.PossibleTargets.Length;

        int index = random.Next(0, maxVal);
        TargetScript newTarget = player.Team.IntelStates.PossibleTargets[index];

        return (target != null && newTarget == target) ? PickRandomNewTarget() : newTarget;
    }
}
