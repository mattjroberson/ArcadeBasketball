using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtificialIntelligence : IntelligenceScript
{
    public override void FixedUpdateIntelligence() { }

    public override void UpdateIntelligence()
    {
        //TODO This is just placeholder for movement 
        player.SetCurrentMoveDirection(Vector2.zero);
    }
}
