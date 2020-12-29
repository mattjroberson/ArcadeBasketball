using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private AttributeSO attributes;
    public AttributeSO Attributes => attributes;
    
    [SerializeField] private PlayerScript teammate;
    public PlayerScript Teammate => teammate;

    [SerializeField] private PlayerScript defender;
    public PlayerScript Defender => defender;

    private TeamScript teamScript;
    public TeamScript Team => teamScript;

    private HandScript hands;
    public Transform Hands => hands.transform;

    private Transform frontPoint;
    public Transform FrontPoint => frontPoint;

    public GoalScript CurrentGoal => Team.getCurrentSide().getGoalScript();
    
    public void Start()
    {
        teamScript = transform.GetComponentInParent<TeamScript>();

        hands = GetComponentInChildren<HandScript>();
        frontPoint = transform.Find("FrontPoint");

        attributes.InitializeAttributes();
    }

    //TODO Maybe this can be moved to a better place.
    //Checks if the player is within a margin from where the rebound is landing
    public bool IsInReboundRange(float rbndFloorY)
    {
        float playerY = frontPoint.position.y;
        float rbndFloorMargin = .4f;

        //If player is above floor - margin and above floor + margin return true
        if (playerY > rbndFloorY - rbndFloorMargin && playerY < rbndFloorY + rbndFloorMargin)
        {
            return true;
        }
        else return false;
    }
}
