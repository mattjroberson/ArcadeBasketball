using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents events;
   
    private void Awake()
    {
        events = this;
    }

    public event Action<PlayerScript> onPossessionChange;
    public void PossessionChange(PlayerScript newBallHandler)
    {
        onPossessionChange?.Invoke(newBallHandler);
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
}
