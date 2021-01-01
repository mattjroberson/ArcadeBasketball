using System;
using UnityEngine;

public class BallEvents : MonoBehaviour
{
    public static BallEvents events;

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

    public event Action<PlayerScript> onBallTouched;
    public void BallTouched(PlayerScript player)
    {
        onBallTouched?.Invoke(player);
    }
}
