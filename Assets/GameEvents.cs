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

    public event Action<PlayerScript> onBallStolen;
    public void BallStolen(PlayerScript defender)
    {
        onBallStolen?.Invoke(defender);
    }

}
