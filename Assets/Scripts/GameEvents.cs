using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents events;
   
    private void Awake()
    {
        events = this;
    }

    public event Action<GoalScript, bool> onBallShot;
    public void BallShot(GoalScript goal, bool madeShot)
    {
        onBallShot?.Invoke(goal, madeShot);
    }

    public event Action<PlayerScript> onBallPassed;
    public void BallPassed(PlayerScript target)
    {
        onBallPassed?.Invoke(target);
    }

    public event Action<PlayerScript> onPassReceived;
    public void PassReceived(PlayerScript receiver)
    {
        onPassReceived?.Invoke(receiver);
    }
    public event Action<PlayerScript> onPassStolen;
    public void PassStolen(PlayerScript receiver)
    {
        onPassStolen?.Invoke(receiver);
    }

    public event Action<PlayerScript> onBallStolen;
    public void BallStolen(PlayerScript defender)
    {
        onBallStolen?.Invoke(defender);
    }

    public event Action<PlayerScript> onLooseBallPickup;
    public void LooseBallPickup(PlayerScript player)
    {
        onLooseBallPickup?.Invoke(player);
    }

    public event Action<PlayerScript> onPossessionChange;
    public void PossessionChange(PlayerScript newBallHandler)
    {
        onPossessionChange?.Invoke(newBallHandler);
    }

}
