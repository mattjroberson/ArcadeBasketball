using System;
using UnityEngine;

public class DunkingPhysics : PlayerPhysicsType
{
    private float[] dunkTrajectory;
    private const float DUNK_ANGLE = Mathf.PI / 3;
    private const float DUNK_ARC_PEAK_PERCENT = .5f;

    private const float STRAIGHT_DUNK_MARGIN = .1f;
    private const float STRAIGHT_DUNK_GRAVITY = 1.5f;

    private float dunkSpeed;

    private bool straightDunking;

    public DunkingPhysics(MovePhysics physics, PlayerScript player, ActionsScript actions) : base(physics, player, actions)
    {
        actions.events.onDunkBegin += Begin;
    }

    public override void Begin()
    {
        dunkTrajectory = CalculateDunkTrajectory();
        straightDunking = dunkTrajectory[0] == 0;

        dunkSpeed = (straightDunking) ? physics.STRAIGHT_DUNK_SPEED : CalculateArcDunkSpeed();
        bool inFront = Position.x < player.Team.DunkTarget.PlayerPos.x;

        CompareX = inFront ? (Func<float, float, bool>) GreaterThan : LessThan;
        CompareY = (straightDunking) ? (Func<float, float, bool>) GreaterThan : LessThan;

        FaceGoalDir();

        physics.SetAirborn(true);
        physics.CurrentState = MovePhysics.MoveState.DUNKING;
    }

    public override void Update()
    {
        Debug.Log(straightDunking);
        Position = (straightDunking) ? MoveStraight() : MoveArc();

        //If player reaches the dunk target, stop dunking
        bool reachedX = CompareX(Position.x, player.Team.DunkTarget.PlayerPos.x);
        bool reachedY = CompareY(Position.y, player.Team.DunkTarget.PlayerPos.y);

        if (reachedX && reachedY) End();
    }

    public override void End()
    {
        player.States.FloorPos = new Vector2(Position.x, player.Team.DriveTarget.FrontPointPos.y);

        physics.SetJumpY(player.Team.DriveTarget.PlayerPos.y);
        physics.CurrentState = MovePhysics.MoveState.JUMPING;
        actions.GetDunkAction().Stop();
    }

    private Vector2 MoveArc()
    {
        float newX = Position.x + (dunkSpeed * Time.deltaTime);
        float newY = (dunkTrajectory[0] * Mathf.Pow(newX, 2)) + (dunkTrajectory[1] * newX) + dunkTrajectory[2];

        return new Vector2(newX, newY);
    }

    private Vector2 MoveStraight()
    {
        dunkSpeed -= (STRAIGHT_DUNK_GRAVITY * Time.deltaTime);

        float newY = Position.y + (dunkSpeed * Time.deltaTime);
        float newX = (newY - dunkTrajectory[2]) / dunkTrajectory[1];

        return new Vector2(newX, newY);
    }

    private float[] CalculateDunkTrajectory()
    {
        float distToDunkX = player.Team.DunkTarget.PlayerPos.x - Position.x;

        if(distToDunkX < STRAIGHT_DUNK_MARGIN) 
            return TrajectoryScript.CalculateStraightTrajectory(Position, player.Team.DunkTarget.PlayerPos);
        else 
            return TrajectoryScript.CalculateTrajectory(Position, player.Team.DunkTarget.PlayerPos, DUNK_ANGLE, DUNK_ARC_PEAK_PERCENT);
    }

    private float CalculateArcDunkSpeed()
    {
        float distToDunkX = player.Team.DunkTarget.PlayerPos.x - Position.x;
        float scalar = distToDunkX / physics.MEAN_DUNK_LENGTH;
        float newSpeed = physics.MEAN_DUNK_X_SPEED * scalar;

        newSpeed = Mathf.Clamp(newSpeed, 0, physics.MAX_DUNK_X_SPEED);

        return newSpeed;
    }

    private void FaceGoalDir()
    {
        physics.HandleOrientation(player.Team.Goal.isRightGoal ? 1 : -1);
    }

    private Func<float, float, bool> CompareX;
    private Func<float, float, bool> CompareY;

    private bool LessThan(float p1, float p2) { return p1 <= p2; }
    private bool GreaterThan(float p1, float p2) { return p1 >= p2; }
}
