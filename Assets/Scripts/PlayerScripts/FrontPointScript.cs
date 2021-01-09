using UnityEngine;

public class FrontPointScript : MonoBehaviour
{
    private PlayerScript player;

    public Vector2 FloorPosition => player.States.IsAirborn ? player.States.FloorPos : (Vector2)transform.position;
    void Start()
    {
        player = GetComponentInParent<PlayerScript>();
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
            player.States.InAutoDunkZone = val;
        }
        else if (other.name == "moving_dunk")
        {
            player.States.InMovingDunkZone = val;
        }
        else if (other.tag == "shotZone")
        {
            if(val == true) player.States.ShotZoneName = other.name;
        }
    }
}
