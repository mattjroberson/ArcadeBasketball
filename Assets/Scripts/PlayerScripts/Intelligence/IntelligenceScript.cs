using UnityEngine;

public abstract class IntelligenceScript : MonoBehaviour
{
    protected ActionsScript actions;
    protected PlayerStateScript playerStates;

    public virtual void Start()
    {
        actions = GetComponentInParent<ActionsScript>();
        playerStates = GetComponentInParent<PlayerStateScript>();

        SetMoveDirection(Vector2.zero);
    }

    public abstract void FixedUpdateIntelligence();

    public abstract void UpdateIntelligence();

    public void SetMoveDirection(Vector2 moveDirection) { playerStates.SetCurrentMoveDirection(moveDirection); }
}
