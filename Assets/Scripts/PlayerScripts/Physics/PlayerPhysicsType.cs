using UnityEngine;

public abstract class PlayerPhysicsType 
{
    protected PlayerScript player;
    protected MovePhysics physics;
    protected ActionsScript actions;

    protected Vector2 Position { get { return physics.Rigidbody.position; } set { physics.Rigidbody.MovePosition(value); } }

    public PlayerPhysicsType(MovePhysics physics, PlayerScript player, ActionsScript actions)
    {
        this.player = player;
        this.physics = physics;
        this.actions = actions;
    }

    public abstract void Begin();

    public abstract void Update();

    public abstract void End();
}
