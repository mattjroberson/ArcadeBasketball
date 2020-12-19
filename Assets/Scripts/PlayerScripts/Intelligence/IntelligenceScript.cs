using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IntelligenceScript : MonoBehaviour
{
    protected PlayerScript player;
    protected PhysicsScript physics;
    protected ActionsScript actions;

    protected  Vector2 moveDirection;

    public abstract void FixedUpdateIntelligence();

    public abstract void UpdateIntelligence();

    public Vector2 GetMoveDirection() { return moveDirection; }
}
