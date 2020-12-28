using UnityEngine;

public class FrontPointScript : MonoBehaviour
{
    private PlayerStateScript playerStates;

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
            playerStates.SetInAuoDunkZone(true);
        }
        else if (other.name == "moving_dunk")
        {
            playerStates.SetInMovingDunkZone(true);
        }
        else if (other.tag == "shotZone")
        {
            playerStates.SetShotZoneName(other.name);
        }
    }
}
