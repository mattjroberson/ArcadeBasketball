using UnityEngine;

public abstract class MovementType 
{
    protected Vector2 currentVelocity;
    protected readonly PhysicsScript physics;
    protected readonly ActionsScript actions;
    protected readonly PlayerScript player;
    protected MovementType(PhysicsScript physics, PlayerScript player, ActionsScript actions)
    {
        this.physics = physics;
        this.player = player;
        this.actions = actions;
    }

    public abstract void Update();
}
