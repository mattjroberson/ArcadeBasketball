using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtificialIntelligence : IntelligenceScript
{
    public override void FixedUpdateIntelligence() { }

    public override void UpdateIntelligence()
    {
        //This is just placeholder for movement 
        playerStates.SetCurrentMoveDirection(Vector2.zero);
    }
}
