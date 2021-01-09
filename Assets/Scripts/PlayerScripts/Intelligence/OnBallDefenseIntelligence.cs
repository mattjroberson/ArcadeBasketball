using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnBallDefenseIntelligence : ArtificialIntelligence
{
    private int i = 0;

    public int gap = 50;

    public OnBallDefenseIntelligence(PlayerScript player, ActionsScript actions) : base(player, actions) { }

    public override void FixedUpdateIntelligence() { }

    public override void Wake() { }

    public override void UpdateIntelligence() {
        i++;
        if (i > gap)
        {
            i = 0;
            //actions.GetJumpAction().Start(1.1f);
        }
    }
}
