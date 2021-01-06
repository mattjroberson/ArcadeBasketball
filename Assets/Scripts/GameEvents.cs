using System;
using UnityEngine;

public sealed class GameEvents
{
    private static readonly GameEvents instance = new GameEvents();

    static GameEvents() { }

    private GameEvents() { }

    public static GameEvents Instance => instance;

    public event Action<PlayerScript> onPossessionChange;
    public void PossessionChange(PlayerScript newBallHandler)
    {
        onPossessionChange?.Invoke(newBallHandler);
    }

    public event Action<PlayerScript> onUserPlayerSwitch;
    public void UserPlayerSwitch(PlayerScript oldUser)
    {
        onUserPlayerSwitch?.Invoke(oldUser);
    }

    public event Action<PlayerScript> onPassSent;
    public void PassSent(PlayerScript receiver)
    {
        onPassSent?.Invoke(receiver);
    }

    public event Action<PlayerScript> onPassReceived;
    public void PassReceived(PlayerScript receiver)
    {
        onPassReceived?.Invoke(receiver);
    }

    public event Action<Vector2> onBallLoose;
    public void BallLoose(Vector2 landingPos)
    {
        onBallLoose?.Invoke(landingPos);
    }
}
