using System;
using System.Collections.Generic;
using UnityEngine;

public class OffBallOffenseIntel : OffenseIntelType
{
    public OffBallOffenseIntel(PlayerScript player, ActionsScript actions) : base(player, actions) { }

    public override void Wake()
    {
        base.Wake();
        player.Team.IntelStates.onBallHandlerPositionUpdated += BallHandlerPositionUpdatedEvent;
        MoveToSpot(PickRandomNewTarget());
    }

    public override void Sleep()
    {
        base.Sleep();
        player.Team.IntelStates.onBallHandlerPositionUpdated -= BallHandlerPositionUpdatedEvent;
    }

    private void BallHandlerPositionUpdatedEvent()
    {
        if (NeedsToMove()) MoveToSpot(PickRandomNewTarget());
    }

    private bool NeedsToMove()
    {
        int check = Array.IndexOf(possibleTargets[player.Team.IntelStates.HandlerTargetName], target);
        return check == -1;
    }

    protected override void MakeDecision()
    {
        MoveToSpot(PickRandomNewTarget());
    }

    protected override Vector2 PickRandomNewTarget()
    {
        Vector2 newTarget = base.PickRandomNewTarget();       
        return (target != null && newTarget == target) ? PickRandomNewTarget() : newTarget;
    }

    protected override void BuildPossibleTargetDict()
    {
        possibleTargets = new Dictionary<string, Vector2[]>();

        possibleTargets.Add("top_corner", new Vector2[] {
            player.Team.Side.OffTargetPoints.Key.FeetPos,
            player.Team.Side.OffTargetPoints.BotWing.FeetPos,
            player.Team.Side.OffTargetPoints.BotCorner.FeetPos});

        possibleTargets.Add("top_wing", new Vector2[] {
            player.Team.Side.OffTargetPoints.BotWing.FeetPos,
            player.Team.Side.OffTargetPoints.BotCorner.FeetPos});

        possibleTargets.Add("key", new Vector2[] {
            player.Team.Side.OffTargetPoints.TopCorner.FeetPos,
            player.Team.Side.OffTargetPoints.BotCorner.FeetPos});

        possibleTargets.Add("bot_wing", new Vector2[] {
            player.Team.Side.OffTargetPoints.TopWing.FeetPos,
            player.Team.Side.OffTargetPoints.TopCorner.FeetPos});

        possibleTargets.Add("bot_corner", new Vector2[] {
            player.Team.Side.OffTargetPoints.Key.FeetPos,
            player.Team.Side.OffTargetPoints.TopWing.FeetPos,
            player.Team.Side.OffTargetPoints.TopCorner.FeetPos});

        possibleTargets.Add("none", new Vector2[] {
            player.Team.Side.OffTargetPoints.BotWing.FeetPos,
            player.Team.Side.OffTargetPoints.BotCorner.FeetPos,
            player.Team.Side.OffTargetPoints.Key.FeetPos,
            player.Team.Side.OffTargetPoints.TopWing.FeetPos,
            player.Team.Side.OffTargetPoints.TopCorner.FeetPos});
    }
}
