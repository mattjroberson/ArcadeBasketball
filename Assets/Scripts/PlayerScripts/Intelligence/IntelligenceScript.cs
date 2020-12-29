using UnityEngine;

public abstract class IntelligenceScript
{
    protected IntelligenceContainer intelligence;
    protected ActionsScript actions;
    protected PlayerStateScript playerStates;

    public IntelligenceScript(IntelligenceContainer intelligence)
    {
        this.intelligence = intelligence;

        actions = intelligence.Actions;
        playerStates = intelligence.PlayerStates;

        SetMoveDirection(Vector2.zero);
    }

    public abstract void FixedUpdateIntelligence();

    public abstract void UpdateIntelligence();

    public void SetMoveDirection(Vector2 moveDirection) { playerStates.CurrentMoveDirection = moveDirection; }
}
