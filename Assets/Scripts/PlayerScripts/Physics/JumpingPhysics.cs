using System;
using UnityEngine;

public class JumpingPhysics : PlayerPhysicsType
{
    private Vector2 jumpVelocity;
    private const float FLOOR_MARGIN = .01f;

    public float JumpY { get; set; }

    public JumpingPhysics(MovePhysics physics, PlayerScript player, ActionsScript actions) : base(physics, player, actions)
    {
        actions.events.onJumpBegin += Begin;
        actions.events.onJumpStop += Stop;
    }

    public override void Begin() { throw new NotImplementedException(); }

    public void Begin(float scalar)
    {
        jumpVelocity = new Vector2(0, player.Attributes.GetMaxJump() * scalar);
        player.States.FloorPos = player.FrontPoint.FloorPosition;
        JumpY = Position.y;

        physics.SetAirborn(true);
        physics.CurrentState = MovePhysics.MoveState.JUMPING;
    }
    public override void Update()
    {
        jumpVelocity += Physics2D.gravity * Time.deltaTime;

        Vector2 distMoved = jumpVelocity * Time.deltaTime;
        Position = Position + distMoved;

        if (Position.y <= (JumpY - FLOOR_MARGIN)) End();
    }

    public void Stop()
    {
        if (jumpVelocity.y > 0) jumpVelocity = Vector2.zero;
    }

    public override void End()
    {
        Position = new Vector2(Position.x, JumpY);

        physics.SetAirborn(false);
        physics.CurrentState = MovePhysics.MoveState.RUNNING;

        actions.GetJumpAction().End();
    }
}
