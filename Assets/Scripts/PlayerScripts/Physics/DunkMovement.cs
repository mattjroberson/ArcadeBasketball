using UnityEngine;

public class DunkMovement : MovementType, IMovementEvent
{
    private float[] dunkTrajectory;
    private float dunkSpeed;
    private float dunkAngle;
    private float dunkArcPeakPercent;

    public DunkMovement(PhysicsScript physics, PlayerScript player, ActionsScript actions) : base(physics, player, actions) 
    {
        //TODO Fix Hardcoded Values
        dunkSpeed = 2.25f;
        dunkAngle = Mathf.PI / 3;
        dunkArcPeakPercent = .5f;
    }

    public void Start()
    {
        physics.SetCurrentMoveState(PhysicsScript.MoveState.DUNKING);
        //TODO Fix Hardcoded value
        dunkTrajectory = TrajectoryScript.CalculateTrajectory(physics.transform.position, new Vector2(5.3f, .2f), dunkAngle, dunkArcPeakPercent);
        physics.HandleLockedElements(false);

        currentVelocity = Vector2.zero;
    }

    public override void Update()
    {
        float newX = physics.transform.position.x + (dunkSpeed * Time.deltaTime);
        float newY = (dunkTrajectory[0] * newX * newX) + (dunkTrajectory[1] * newX) + dunkTrajectory[2];

        physics.transform.position = new Vector2(newX, newY);

        //TODO Fix Hardcoded Values
        //If player reaches the dunk target, stop dunking
        if (physics.transform.position.x >= 5.3f && physics.transform.position.y <= .2f)
        {
            Stop();
        }
    }

    public void Stop()
    {
        //TODO Fix Hardcoded Values
        physics.SetCurrentMoveState(PhysicsScript.MoveState.JUMPING);
        jumpFloor = new Vector2(physics.transform.position.x, -.8f);
        actions.GetDunkAction().Stop();
    }
}
