using System;

public class TeamIntelStates
{   
    public string HandlerTargetName { get; private set; } = "none";

    public TeamIntelStates()
    {
    }

    public event Action onBallHandlerPositionUpdated;
    public void BallHandlerPositionUpdated(string newTargetName)
    {
        HandlerTargetName = newTargetName;
        onBallHandlerPositionUpdated?.Invoke();
    }
}
