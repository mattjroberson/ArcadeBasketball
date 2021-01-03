using UnityEngine;

public class FrontPointScript : MonoBehaviour
{
    private PlayerStateScript playerStates;

    public Vector2 FloorPosition => playerStates.IsAirborn ? playerStates.FloorPos : (Vector2)transform.position;
    void Start()
    {
        playerStates = GetComponentInParent<PlayerStateScript>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IdentifyAndSetFlag(other, true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IdentifyAndSetFlag(other, false);
    }

    private void IdentifyAndSetFlag(Collider2D other, bool val)
    {
        if (other.name == "auto_dunk")
        {
            playerStates.InAutoDunkZone = val;
        }
        else if (other.name == "moving_dunk")
        {
            playerStates.InMovingDunkZone = val;
        }
        else if (other.tag == "shotZone")
        {
            if(val == true) playerStates.ShotZoneName = other.name;
        }
    }
}
