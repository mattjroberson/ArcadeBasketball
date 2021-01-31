using UnityEngine;

public abstract class IntelligenceType
{
    protected PlayerScript player;
    protected ActionsScript actions;
    protected bool awake;

    public IntelligenceType(PlayerScript player, ActionsScript actions)
    {
        this.player = player;
        this.actions = actions;

        player.States.CurrentMoveDirection = Vector2.zero;
    }

    public abstract void UpdateIntelligence();

    public virtual void Wake() { awake = true; }

    public virtual void Sleep() { awake = false; }
}
