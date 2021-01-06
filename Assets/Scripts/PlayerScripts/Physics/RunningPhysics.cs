using UnityEngine;

public class RunningPhysics : PlayerPhysicsType
{
    private float currentSpeed;

    public RunningPhysics(MovePhysics physics, PlayerScript player, ActionsScript actions) : base(physics, player, actions)
    {
        currentSpeed = player.Attributes.GetMaxSpeed();

        actions.events.onSprintBegin += Begin;
        actions.events.onSprintEnd += End;
    }

    public override void Begin()
    {
        currentSpeed *= player.Attributes.GetSprintBonus();
    }

    public override void Update()
    {
        if (player.States.IsFrozen) return;

        Vector2 currentPos = Position;

        Vector2 inputVector = player.States.CurrentMoveDirection;
        inputVector = Vector2.ClampMagnitude(inputVector, 1);

        Vector2 movement = inputVector * currentSpeed;
        Vector2 newPos = currentPos + movement * Time.fixedDeltaTime;

        physics.HandleOrientation(movement.x);
        Position = newPos;
    }

    public override void End()
    {
        currentSpeed = player.Attributes.GetMaxSpeed();
    }
}
