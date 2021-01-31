using System;
using UnityEngine;

public class BallEvents
{
    private static readonly BallEvents instance = new BallEvents();

    static BallEvents() { }

    private BallEvents() { }

    public static BallEvents Instance => instance;

    public event Action<GoalScript, bool> onBallShot;
    public void BallShot(GoalScript goal, bool madeShot)
    {
        onBallShot?.Invoke(goal, madeShot);
    }

    public event Action<GoalScript, bool> onDunkAttempt;
    public void DunkAttempt(GoalScript goal, bool madeDunk)
    {
        onDunkAttempt?.Invoke(goal, madeDunk);
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

    public event Action<PlayerScript> onBallTouchedHand;
    public void BallTouchedHand(PlayerScript player)
    {
        onBallTouchedHand?.Invoke(player);
    }

    public event Action<PlayerScript> onBallTouchedFoot;
    public void BallTouchedFoot(PlayerScript player)
    {
        onBallTouchedFoot?.Invoke(player);
    }
}
