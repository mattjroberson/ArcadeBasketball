using UnityEngine;

public class SideManager : MonoBehaviour
{
    [SerializeField] private TargetScript dunkTarget;
    public TargetScript DunkTarget => dunkTarget;

    [SerializeField] private TargetScript driveTarget;
    public TargetScript DriveTarget => driveTarget;

    [SerializeField] private GoalScript goalScript;
    public GoalScript Goal => goalScript;
}
