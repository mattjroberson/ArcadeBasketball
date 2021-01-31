using UnityEngine;

public class FrontPointScript : MonoBehaviour
{
    private ActionsScript actions;

    private void Start()
    {
        actions = GetComponentInParent<ActionsScript>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("aiZone")) {
            actions.TouchAiZone(other.name);
        }
        else if(other.CompareTag("trackingZone") == false)
            actions.SetShotZoneFlag(other, true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("trackingZone") == false) {
            actions.SetShotZoneFlag(other, false);
        }
    }
}
