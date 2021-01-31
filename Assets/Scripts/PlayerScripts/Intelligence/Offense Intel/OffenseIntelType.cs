using System.Collections.Generic;
using UnityEngine;

public abstract class OffenseIntelType : ArtificialIntelType
{
    protected const float DECISION_WINDOW = 3f;
    protected float decisionTimer = 0;
    
    protected Dictionary<string, Vector2[]> possibleTargets;
    protected readonly System.Random random = new System.Random();

    public OffenseIntelType(PlayerScript player, ActionsScript actions) : base(player, actions)
    {
        BuildPossibleTargetDict();
    }

    public override void UpdateIntelligence()
    {
        base.UpdateIntelligence();

        if (state == IntelState.IDLE) {
            decisionTimer += Time.deltaTime;

            if (decisionTimer > DECISION_WINDOW) {
                decisionTimer = 0;
                MakeDecision();
            }
        }
    }

    public override void Wake()
    {
        base.Wake();
        actions.events.onAiZoneTouched += AiZoneTouchedEvent;
    }

    public override void Sleep()
    {
        base.Sleep();
        actions.events.onAiZoneTouched -= AiZoneTouchedEvent;
    }

    protected virtual Vector2 PickRandomNewTarget()
    {
        Vector2[] targets = possibleTargets[player.Team.IntelStates.HandlerTargetName];

        int maxVal = targets.Length;
        int index = random.Next(0, maxVal);

        return targets[index];
    }

    protected abstract void MakeDecision();

    protected abstract void BuildPossibleTargetDict();

    private void AiZoneTouchedEvent(string name)
    {
        if(player.States.HasBall) player.Team.IntelStates.BallHandlerPositionUpdated(name);
    }
}
