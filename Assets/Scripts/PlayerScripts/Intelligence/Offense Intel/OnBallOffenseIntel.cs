using System.Collections.Generic;
using UnityEngine;

public class OnBallOffenseIntel : OffenseIntelType
{
    public OnBallOffenseIntel(PlayerScript player, ActionsScript actions) : base(player, actions) {
        BuildPossibleTargetDict();
    }

    public override void Wake() {
        MoveToSpot(PickRandomNewTarget());
        base.Wake(); 
    }

    protected override void MakeDecision()
    {
        //TODO Wrap this logic in a brain class
        int i = random.Next(0, 10);

        if (i < 5) MoveToSpot(PickRandomNewTarget());
        else actions.StartPassing();
        ///////////////////////////////////
    }

    protected override void BuildPossibleTargetDict()
    {
        possibleTargets = new Dictionary<string, Vector2[]>();

        possibleTargets.Add("top_corner", new Vector2[] {
            player.Team.Side.OffTargetPoints.TopWing.FeetPos});

        possibleTargets.Add("top_wing", new Vector2[] {
            player.Team.Side.OffTargetPoints.TopCorner.FeetPos,
            player.Team.Side.OffTargetPoints.Key.FeetPos});

        possibleTargets.Add("key", new Vector2[] {
            player.Team.Side.OffTargetPoints.TopWing.FeetPos,
            player.Team.Side.OffTargetPoints.BotWing.FeetPos});

        possibleTargets.Add("bot_wing", new Vector2[] {
            player.Team.Side.OffTargetPoints.Key.FeetPos,
            player.Team.Side.OffTargetPoints.BotCorner.FeetPos});

        possibleTargets.Add("bot_corner", new Vector2[] {
            player.Team.Side.OffTargetPoints.BotWing.FeetPos});

        possibleTargets.Add("none", new Vector2[] {
            player.Team.Side.OffTargetPoints.BotWing.FeetPos,
            player.Team.Side.OffTargetPoints.BotCorner.FeetPos,
            player.Team.Side.OffTargetPoints.Key.FeetPos,
            player.Team.Side.OffTargetPoints.TopWing.FeetPos,
            player.Team.Side.OffTargetPoints.TopCorner.FeetPos});
    }
}
