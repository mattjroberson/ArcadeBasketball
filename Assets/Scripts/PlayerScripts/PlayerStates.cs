using UnityEngine;

public class PlayerStates
{
    private PlayerScript player;
    public bool InMovingDunkZone { get; set; }
    public bool InAutoDunkZone { get; set; }
    public bool InDunkRange => InAutoDunkZone || (InMovingDunkZone && MovingDunkCheck);

    public string ShotZoneName { get; set; }
    public bool HasBall { get; set; }
    public bool IsAirborn { get; set; }
    public bool IsFrozen { get; set; }
    public bool IsOffense { get; }
    public Vector2 CurrentMoveDirection { get; set; }
    public Vector2 FloorPosAirborn { get; set; }
    public Vector2 FloorPosition => player.States.IsAirborn ? player.States.FloorPosAirborn : (Vector2)player.FrontPointTransform.position;
    public Vector2 FeetPosition => new Vector2(player.transform.position.x, FloorPosition.y);


    public PlayerStates(PlayerScript player)
    {
        this.player = player;

        CheckForPossession();

        CurrentMoveDirection = Vector2.zero;
        IsFrozen = false;
        IsAirborn = false;
    }

    private void CheckForPossession()
    {
        HasBall = (player.transform.GetComponentInChildren<BallScript>() != null);
    }

    private bool MovingDunkCheck =>
        (player.Team.Side.Goal.isRightGoal) ? (CurrentMoveDirection.x > 0) : (CurrentMoveDirection.x < 0);
}
