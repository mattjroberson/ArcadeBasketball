using UnityEngine;

public class PlayerStateScript : MonoBehaviour
{
    private PlayerScript player;

    private Vector2 currentMoveDirection;

    private bool isOffense;
    private bool hasBall;
    private bool isFrozen;

    private string shotZoneName;

    private bool inMovingDunkZone;
    private bool inAutoDunkZone;

    public void Start()
    {
        player = GetComponent<PlayerScript>();

        CheckForPossession();

        currentMoveDirection = Vector2.zero;
        isOffense = false;
        isFrozen = false;
    }

    private void CheckForPossession()
    {
        //If player has the baskeball as a child, set true
        if (player.transform.GetComponentInChildren<BallScript>() != null)
        {
            hasBall = true;
        }
        else hasBall = false;
    }

    //TODO Make this event driven
    public void SetHasBall(bool newHasBall) { hasBall = newHasBall; }

    //TODO Make this event driven
    public void SetFrozen(bool isFrozen) { this.isFrozen = isFrozen; }

    public Vector2 GetCurrentMoveDirection() { return currentMoveDirection; }

    public void SetCurrentMoveDirection(Vector2 currentMoveDirection) { this.currentMoveDirection = currentMoveDirection; }

    public string GetShotZoneName() { return shotZoneName; }

    public void SetShotZoneName(string name) { this.shotZoneName = name; }

    public void SetInMovingDunkZone(bool inMovingDunkZone) { this.inMovingDunkZone = inMovingDunkZone; }

    public void SetInAuoDunkZone(bool inAutoDunkZon) { this.inAutoDunkZone = inAutoDunkZon; }

    public bool GetInDunkRange() { return inAutoDunkZone || MovingDunkCheck(); }

    private bool MovingDunkCheck()
    {
        if (inMovingDunkZone == false) return false;

        return (player.GetGoal().isRightGoal) ? (currentMoveDirection.x > 0) : (currentMoveDirection.x < 0);
    }

    public bool IsOffense() { return isOffense; }

    public bool GetHasBall() { return hasBall; }

    public bool IsFrozen() { return isFrozen; }

}
