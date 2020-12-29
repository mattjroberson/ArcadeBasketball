﻿using UnityEngine;

public class PlayerStateScript : MonoBehaviour
{
    private PlayerScript player;
    
    public bool InMovingDunkZone { get; set; }
    public bool InAutoDunkZone { get; set; }
    public bool InDunkRange => InAutoDunkZone || (InMovingDunkZone && MovingDunkCheck);

    public string ShotZoneName { get; set; }
    public bool HasBall { get; set; }
    public bool IsFrozen { get; set; }
    public bool IsOffense { get; }
    public Vector2 CurrentMoveDirection { get; set; }

    public void Start()
    {
        player = GetComponent<PlayerScript>();

        CheckForPossession();

        CurrentMoveDirection = Vector2.zero;
        IsFrozen = false;
    }

    private void CheckForPossession()
    {
        HasBall = (player.transform.GetComponentInChildren<BallScript>() != null);
    }

    private bool MovingDunkCheck =>
        (player.CurrentGoal.isRightGoal) ? (CurrentMoveDirection.x > 0) : (CurrentMoveDirection.x < 0);
}
