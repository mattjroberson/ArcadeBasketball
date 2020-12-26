using UnityEngine;

public class JumpMovement : MovementType, IMovementEvent
{
    private Vector2 jumpFloor;

    public JumpMovement(PhysicsScript physics, PlayerScript player, ActionsScript actions) : base(physics, player, actions) { }

    public void Start()
    {
        physics.SetCurrentMoveState(PhysicsScript.MoveState.JUMPING);

        jumpFloor = physics.transform.position;
        physics.HandleLockedElements(false);

        currentVelocity = new Vector2(0, player.GetAttributes().GetMaxJump());
    }

    public void Stop()
    {
        physics.transform.position = jumpFloor;
        physics.HandleLockedElements(true);

        physics.SetCurrentMoveState(PhysicsScript.MoveState.RUNNING);
        actions.GetJumpAction().Stop();
    }

    public override void Update()
    {
        //Actually update the position
        currentVelocity += Physics2D.gravity * Time.deltaTime;

        Vector2 movement = currentVelocity * Time.deltaTime;
        physics.transform.position = physics.transform.position + (Vector3)movement;

        //If player back on ground, stop jumping
        if (physics.transform.position.y <= jumpFloor.y)
        {
            Stop();
        }
    }
}
