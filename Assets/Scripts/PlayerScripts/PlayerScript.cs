using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField]
    private AttributeSO attributes;
    [SerializeField]
    private PlayerScript teammate;

    private TeamScript teamScript;

    private PhysicsScript physics;
    
    private IntelligenceContainer intelligence;

    private HandScript hands;
    private Transform frontPoint;  

    //Internal Values
    private Vector2 currentMoveDirection;
    private float currentSpeed;
    private int stealAttempts;

    private float stealAttemptTimer;

    private bool isOffense;
    private bool facingRight;
    private bool hasBall;
    private bool isFrozen;

    private string shotZoneName;

    public void Awake()
    {
        InitializeValues();
    }
    
    public void Start()
    {
        teamScript = transform.GetComponentInParent<TeamScript>();

        physics = GetComponentInChildren<PhysicsScript>();
        intelligence = GetComponentInChildren<IntelligenceContainer>();

        hands = GetComponentInChildren<HandScript>();
        frontPoint = transform.Find("FrontPoint");

        attributes.InitializeAttributes();
        currentSpeed = attributes.GetMaxSpeed();

        CheckForPossession();
    }

    public void Update()
    {
        //Handles the players intelligence
        intelligence.Current().UpdateIntelligence();

        //Handles the players steal attempts
        UpdateStealLogic();
    }

    public void LateUpdate()
    {
        physics.UpdatePhysics();
    }

    //Adjusts the current speed to sprint or not
    public void HandleSprintSpeed(bool isSprinting)
    {
        if (isSprinting == true) {
            currentSpeed *= attributes.GetSprintBonus();
        }
        else {
            currentSpeed = attributes.GetMaxSpeed();
        }
    }

    //Update the possession and players intelligence, called from GameLogic
    public void HandlePossession(bool newHasBall, IntelligenceContainer.IntelligenceType intelType)
    {
        hasBall = newHasBall;
        intelligence.SetIntelligenceType(intelType);
    }

    //Check to see if the player is going to make the shot
    public bool CalculateMadeShot()
    {
        //Get the players chance of making the shot from the zone he's in
        float shotPercentage = attributes.GetShotPercentage(GetShotZoneName());

        //Random value between 0 - 1
        float chance = Random.value;

        //If chance is within shotPercentage, return true
        if (chance <= shotPercentage) return true;
        else return false;
    }

    //Checks if the player is within a margin from where the rebound is landing
    public bool IsInReboundRange(float rbndFloorY)
    {
        float playerY = frontPoint.position.y;
        float rbndFloorMargin = .4f;

        //If player is above floor - margin and above floor + margin return true
        if (playerY > rbndFloorY - rbndFloorMargin && playerY < rbndFloorY + rbndFloorMargin) {
            return true;
        }
        else return false;
    }

    //Handles the counting and resetting of steal attempts
    private void UpdateStealLogic()
    {
        //Dont bother checking if there are no steal attempts
        if (stealAttempts == 0) return;

        stealAttemptTimer += Time.deltaTime;

        //Keeps tally of how many steal attempts in a given time slot, if too many, fouls can be drawn
        if (stealAttemptTimer > attributes.GetStealAttemptWindow()) {
            stealAttempts = 0;
            stealAttemptTimer = 0f;
        }
    }
    
    //Handles possession at initialization phase
    private void CheckForPossession()
    {
        //If player has the baskeball as a child, set true
        if (transform.GetComponentInChildren<BallScript>() != null) {
            hasBall = true;
        }
        else hasBall = false;
    }

    //Handles whether a foul occurred on a steal attempt
    public bool CheckForStealFoul()
    {
        stealAttempts++;

        //If the player has attempted to steal too much
        if (stealAttempts > attributes.GetStealAttemptLimit()) {
            //If the probability results in a foul, return true
            if (Random.value <= attributes.GetStealFoulProbability()) {
                Debug.Log("Player Fouled");
                return true;
            }
        }

        //Return false if no foul
        return false;
    }

    public string GetShotZoneName()
    {
        foreach (PolygonCollider2D zone in teamScript.getCurrentSide().getShotZones().GetComponentsInChildren<PolygonCollider2D>()) {
            if (zone.OverlapPoint(frontPoint.position)) {
                Debug.Log(zone.name);
                return zone.name;
            }
        }

        return null;
    }

    private void InitializeValues()
    {
        currentMoveDirection = Vector2.zero;
        isOffense = false;
        facingRight = true;
        isFrozen = false;
    }

    public float GetCurrentSpeed() { return currentSpeed; }

    public Vector2 GetCurrentMoveDirection() { return currentMoveDirection; }

    public void SetCurrentMoveDirection(Vector2 currentMoveDirection) { this.currentMoveDirection = currentMoveDirection;  }

    public bool IsOffense() { return isOffense; }

    public bool IsFrozen() { return isFrozen; }

    public void SetFrozen(bool isFrozen) { this.isFrozen = isFrozen; }

    public void SetHasBall(bool newHasBall) { hasBall = newHasBall; }

    public bool GetHasBall() { return hasBall; }

    public bool GetFacingRight() { return facingRight; }

    public void SetFacingRight(bool facingRight) { this.facingRight = facingRight; }

    public PlayerScript GetTeammate() { return teammate; }

    public Transform GetHands() { return hands.transform; }

    public Transform GetFrontPoint() { return frontPoint; }

    public GoalScript GetGoal() { return teamScript.getCurrentSide().getGoalScript(); }

    public void SetShotZoneName(string newShotZoneName) { shotZoneName = newShotZoneName; }

    public AttributeSO GetAttributes()
    {
        return attributes;
    }
}
