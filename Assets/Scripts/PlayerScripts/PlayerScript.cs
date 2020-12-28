using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField]
    private AttributeSO attributes;
    [SerializeField]
    private PlayerScript teammate;

    private TeamScript teamScript;

    private IntelligenceContainer intelligence;
    private PlayerStateScript states;

    private HandScript hands;
    private Transform frontPoint;

    public void Start()
    {
        teamScript = transform.GetComponentInParent<TeamScript>();
        intelligence = GetComponentInChildren<IntelligenceContainer>();
        states = GetComponent<PlayerStateScript>();

        hands = GetComponentInChildren<HandScript>();
        frontPoint = transform.Find("FrontPoint");

        attributes.InitializeAttributes();
    }

    public void Update()
    {
        //Handles the players intelligence
        intelligence.Current().UpdateIntelligence();
    }

    //TODO Make this event driven 
    public Transform GetHands() { return hands.transform; }

    //TODO Make this Event Driven
    //Update the possession and players intelligence, called from GameLogic
    public void HandlePossession(bool newHasBall, IntelligenceContainer.IntelligenceType intelType)
    {
        states.SetHasBall(newHasBall);
        intelligence.SetIntelligenceType(intelType);
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

    public PlayerScript GetTeammate() { return teammate; }

    public AttributeSO GetAttributes()
    {
        return attributes;
    }

    public Transform GetFrontPoint() { return frontPoint; }

    public GoalScript GetGoal() { return teamScript.getCurrentSide().getGoalScript(); }
}
