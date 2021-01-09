using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArtificialIntelligence : IntelligenceScript
{
    public ArtificialIntelligence(PlayerScript player, ActionsScript actions) : base(player, actions) { }

    public override void FixedUpdateIntelligence() { }

    public override void UpdateIntelligence()
    {
        //This is just placeholder for movement 
        //player.States.CurrentMoveDirection = Vector2.zero;
    }
}
