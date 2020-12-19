using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public enum SpriteType { DEFAULT, JUMPING, REACHING, DUNKING }

    private GameLogicScript gameLogic;

    private TeamScript teamScript;

    private PhysicsScript physics;
    private IntelligenceContainer intelligence;
    private ActionsScript actions;
    private AttributeScript attributes;
    private SpriteRenderer spriteRenderer;

    private HandScript hands;
    private Transform frontPoint;

    private GameObject shotMeterPrefab;
    private ShotMeter shotMeter;

    public Sprite defaultSprite;
    public Sprite jumpingSprite;
    public Sprite reachingSprite;
    public Sprite dunkingSprite;

    public PlayerScript teammate;
    //public GameObject shotZones;
    //public GoalScript currentGoal;

    //Internal Values
    private float currentSpeed;
    private int stealAttempts;

    private float stealAttemptTimer;

    private bool isOffense;
    private bool facingRight;
    private bool hasBall;

    private string shotZoneName;

    public void Awake()
    {
        InitializeValues();
    }

    public void Start()
    {
        gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogicScript>();

        teamScript = transform.GetComponentInParent<TeamScript>();

        actions = GetComponent<ActionsScript>();
        physics = GetComponentInChildren<PhysicsScript>();
        intelligence = GetComponentInChildren<IntelligenceContainer>();
        attributes = GetComponent<AttributeScript>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        hands = GetComponentInChildren<HandScript>();
        frontPoint = transform.Find("FrontPoint");

        shotMeterPrefab = Resources.Load("Prefabs/ShotMeterPrefab") as GameObject;

        currentSpeed = attributes.GetMaxSpeed();

        SetSprite(SpriteType.DEFAULT);

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

    public void SetSprite(SpriteType type)
    {
        switch (type) {
            case SpriteType.DEFAULT:
                spriteRenderer.sprite = defaultSprite;
                break;
            case SpriteType.JUMPING:
                spriteRenderer.sprite = jumpingSprite;
                break;
            case SpriteType.REACHING:
                spriteRenderer.sprite = reachingSprite;
                StartCoroutine(PutHandsDown());
                break;
            case SpriteType.DUNKING:
                spriteRenderer.sprite = dunkingSprite;
                break;
        }
    }

    //Makes sure the player is facing the correct way
    public void HandleOrientation(float xMovement)
    {
        float rotation = transform.rotation.y;

        bool needsFlip = (rotation == 0) ? (xMovement < 0) : (xMovement > 0);

        if (needsFlip) {
            rotation = (rotation == 0) ? 180f : 0f;
            transform.rotation = Quaternion.AngleAxis(rotation, Vector3.up);

            //Set a variable to track it other places
            facingRight = transform.rotation.y == 0 ? true : false;
        }
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

    //Assumes only two players per team
    private PlayerScript InitTeammate()
    {
        foreach(PlayerScript player in transform.parent.GetComponentsInChildren<PlayerScript>()) {
            if(player.Equals(this) == false) {
                return player;
            }
        }

        return null;
    }

    //Instantiate the shot meter prefab and return the script
    public ShotMeter InstantiateShotMeter()
    {
       shotMeter =  Instantiate(shotMeterPrefab, transform.Find("MeterContainer").position, Quaternion.identity).GetComponentInChildren<ShotMeter>();
       return shotMeter;
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
        isOffense = false;
        facingRight = true;
    }

    private IEnumerator PutHandsDown()
    {
        yield return new WaitForSeconds(.25f);
        SetSprite(SpriteType.DEFAULT);
    }

    public float GetCurrentSpeed() { return currentSpeed; }

    public bool IsOffense() { return isOffense; }

    public void SetHasBall(bool newHasBall) { hasBall = newHasBall; }

    public bool GetHasBall() { return hasBall; }

    public bool GetFacingRight() { return facingRight; }

    public ShotMeter GetShotMeter() { return shotMeter; }

    public PlayerScript GetTeammate() { return teammate; }

    public Transform GetHands() { return hands.transform; }

    public Transform GetFrontPoint() { return frontPoint; }

    public GoalScript GetGoal() { return teamScript.getCurrentSide().getGoalScript(); }

    public void SetShotZoneName(string newShotZoneName) { shotZoneName = newShotZoneName; }
}
