using UnityEngine;

public abstract class IntelligenceScript : MonoBehaviour
{
    protected PlayerScript player;
    protected ActionsScript actions;

    public virtual void Start()
    {
        actions = transform.parent.GetComponentInParent<ActionsScript>();
        player = transform.parent.GetComponentInParent<PlayerScript>();

        SetMoveDirection(Vector2.zero);
    }

    public abstract void FixedUpdateIntelligence();

    public abstract void UpdateIntelligence();

    public void SetMoveDirection(Vector2 moveDirection) { player.SetCurrentMoveDirection(moveDirection); }
}
