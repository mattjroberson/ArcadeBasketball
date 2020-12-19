using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffBallDefenseIntelligence : IntelligenceScript
{
    public void Awake()
    {
        moveDirection = Vector2.zero;
    }

    public void Start()
    {
        actions = transform.parent.GetComponentInParent<ActionsScript>();
    }

    public override void FixedUpdateIntelligence() { }

    public override void UpdateIntelligence() { }
}
