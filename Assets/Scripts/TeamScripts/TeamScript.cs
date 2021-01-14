using UnityEngine;

public class TeamScript : MonoBehaviour
{
    public enum CourtSide { LEFT, RIGHT };
    [SerializeField] private CourtSide currentSide;

    public SideManager Side { get; private set; }

    public TeamIntelStates IntelStates { get; private set; }

    [SerializeField]
    private bool userControlled;
    public bool UserControlled => userControlled;

    private void Awake()
    {
        string side = currentSide == CourtSide.LEFT ? "LeftSide" : "RightSide";
        Side = GameObject.Find(side).GetComponent<SideManager>();
        IntelStates = new TeamIntelStates();
    }
}
