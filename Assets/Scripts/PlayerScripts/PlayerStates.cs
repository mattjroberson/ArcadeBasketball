using UnityEngine;

public class PlayerStates
{
    private PlayerScript player;
    public bool InMovingDunkZone { get; set; }
    public bool InAutoDunkZone { get; set; }
    public bool InDunkRange => InAutoDunkZone || (InMovingDunkZone && MovingDunkCheck);

    public string ShotZoneName { get; set; }
    public bool HasBall { get; set; }
    public bool WaitingOnPass { get; set; }
    public bool IsAirborn { get; set; }
    public bool IsFrozen => WaitingOnPass;
    public bool IsOffense { get; }
    public Vector2 CurrentMoveDirection { get; set; }
    public Vector2 FloorPos { get; set; }

    public PlayerStates(PlayerScript player)
    {
        this.player = player;

        CheckForPossession();

        CurrentMoveDirection = Vector2.zero;
        WaitingOnPass = false;
        IsAirborn = false;
    }

    private void CheckForPossession()
    {
        HasBall = (player.transform.GetComponentInChildren<BallScript>() != null);
    }

    private bool MovingDunkCheck =>
        (player.Team.Side.Goal.isRightGoal) ? (CurrentMoveDirection.x > 0) : (CurrentMoveDirection.x < 0);
}
