using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArtificialIntelligence : IntelligenceScript
{
    public ArtificialIntelligence(IntelligenceContainer intelligence) : base(intelligence) { }

    public override void FixedUpdateIntelligence() { }

    public override void UpdateIntelligence()
    {
        //This is just placeholder for movement 
        intelligence.PlayerStates.CurrentMoveDirection = Vector2.zero;
    }
}
