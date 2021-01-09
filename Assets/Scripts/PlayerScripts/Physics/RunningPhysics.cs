using UnityEngine;

public class RunningPhysics : PlayerPhysicsType
{
    bool sprinting = false;

    public RunningPhysics(MovePhysics physics, PlayerScript player, ActionsScript actions) : base(physics, player, actions)
    {
        actions.events.onSprintBegin += Begin;
        actions.events.onSprintEnd += End;
    }

    public override void Begin()
    {
        sprinting = true;
    }

    public override void Update()
    {
        if (player.States.IsFrozen) return;

        Vector2 currentPos = Position;

        Vector2 inputVector = player.States.CurrentMoveDirection;
        inputVector = Vector2.ClampMagnitude(inputVector, 1);

        float speed = CalculateSpeed();

        Vector2 movement = inputVector * speed;
        Vector2 newPos = currentPos + movement * Time.fixedDeltaTime;

        physics.HandleOrientation(movement.x);
        Position = newPos;
    }

    public override void End()
    {
        sprinting = false;
    }

    private float CalculateSpeed()
    {
        float speed = player.Attributes.MaxSpeed.Value;
        if (sprinting) speed *= player.Attributes.MaxSpeed.Value;

        return speed;
    }
}
