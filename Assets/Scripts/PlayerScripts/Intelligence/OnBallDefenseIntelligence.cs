using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnBallDefenseIntelligence : IntelligenceScript
{
    private int i = 0;

    public int gap = 50;

    public void Awake()
    {
        moveDirection = Vector2.zero;
    }

    public void Start()
    {
        actions = transform.parent.GetComponentInParent<ActionsScript>();
    }

    public override void FixedUpdateIntelligence() { }

    public override void UpdateIntelligence() {
        //i++;
        //if (i > gap) {
        //    i = 0;
        //    actions.StartJumping();
        //}
    }
}
