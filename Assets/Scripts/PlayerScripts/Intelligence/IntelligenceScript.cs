using UnityEngine;

public abstract class IntelligenceScript
{
    protected PlayerScript player;
    protected ActionsScript actions;

    public IntelligenceScript(PlayerScript player, ActionsScript actions)
    {
        this.player = player;
        this.actions = actions;

        player.States.CurrentMoveDirection = Vector2.zero;
    }

    public abstract void FixedUpdateIntelligence();

    public abstract void UpdateIntelligence();

    public abstract void Wake();
}
