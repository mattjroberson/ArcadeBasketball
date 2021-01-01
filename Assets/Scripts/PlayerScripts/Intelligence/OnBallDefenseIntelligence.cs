using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnBallDefenseIntelligence : ArtificialIntelligence
{
    private int i = 0;

    public int gap = 50;

    public OnBallDefenseIntelligence(IntelligenceContainer intelligence) : base(intelligence) { }

    public override void FixedUpdateIntelligence() { }

    public override void UpdateIntelligence() {
        i++;
        if (i > gap)
        {
            i = 0;
            actions.GetJumpAction().Start(1.1f);
        }
    }
}
