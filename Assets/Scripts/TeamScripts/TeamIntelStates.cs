using System;
using System.Collections.Generic;
using UnityEngine;

public class TeamIntelStates
{
    public TargetScript[] PossibleTargets { get { return possibleTargets[team.DebugTarget]; } }
    private Dictionary<TargetScript, TargetScript[]> possibleTargets;

    private TeamScript team;

    public TeamIntelStates(TeamScript team)
    {
        this.team = team;

        BuildPossibleTargetDicts();
    }

    public event Action onBallHandlerPositionUpdated;
    public void BallHandlerPositionUpdated()
    {
        onBallHandlerPositionUpdated?.Invoke();
    }

    private void BuildPossibleTargetDicts()
    {
        possibleTargets = new Dictionary<TargetScript, TargetScript[]>();

        possibleTargets.Add(team.Side.OffTargetPoints.TopCorner, new TargetScript[] {
            team.Side.OffTargetPoints.Key,
            team.Side.OffTargetPoints.BotWing,
            team.Side.OffTargetPoints.BotCorner});

        possibleTargets.Add(team.Side.OffTargetPoints.TopWing, new TargetScript[] {
            team.Side.OffTargetPoints.BotWing,
            team.Side.OffTargetPoints.BotCorner});

        possibleTargets.Add(team.Side.OffTargetPoints.Key, new TargetScript[] {
            team.Side.OffTargetPoints.TopCorner,
            team.Side.OffTargetPoints.BotCorner});

        possibleTargets.Add(team.Side.OffTargetPoints.BotWing, new TargetScript[] {
            team.Side.OffTargetPoints.TopWing,
            team.Side.OffTargetPoints.TopCorner});

        possibleTargets.Add(team.Side.OffTargetPoints.BotCorner, new TargetScript[] {
            team.Side.OffTargetPoints.Key,
            team.Side.OffTargetPoints.TopWing,
            team.Side.OffTargetPoints.TopCorner});

    }
}
