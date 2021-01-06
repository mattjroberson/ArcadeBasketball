using UnityEngine;

public class TeamScript : MonoBehaviour
{
    public enum CourtSide { LEFT, RIGHT };
    [SerializeField] private CourtSide currentSide;

    private SideManager sideManager;

    public TargetScript DunkTarget => sideManager.DunkTarget;
    public TargetScript DriveTarget => sideManager.DriveTarget;

    public GoalScript Goal => sideManager.Goal;

    [SerializeField]
    private bool userControlled;
    public bool UserControlled => userControlled;

    void Awake()
    {
        string side = currentSide == CourtSide.LEFT ? "LeftSide" : "RightSide";
        sideManager = GameObject.Find(side).GetComponent<SideManager>();
    }
}
