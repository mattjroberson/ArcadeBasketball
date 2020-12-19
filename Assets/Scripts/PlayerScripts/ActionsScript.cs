using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionsScript : MonoBehaviour
{
    private GameLogicScript gameLogic;
    private PlayerScript player;
    private PhysicsScript physics;
    private AttributeScript attributes;
    private EnduranceBar endurance;
    private HandScript hands;

    private bool isShooting;
    private bool isDunking;
    private bool isJumping;
    private bool isSprinting;
    private bool isFrozen;

    public void Start()
    {
        gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogicScript>();

        player = GetComponent<PlayerScript>();
        physics = GetComponent<PhysicsScript>();
        attributes = GetComponent<AttributeScript>();
        endurance = GetComponentInChildren<EnduranceBar>();

        hands = GetComponentInChildren<HandScript>();
    }

    public void HandleShotType()
    {
        //Face the goal
        FaceTarget(player.GetGoal().transform);

        //Dunk if dunking, 
        if (physics.ShouldDunk()) {
            StartDunking();
        }
        //Shoot if shooting
        else {
            StartShooting();
        }  
    }

    public void ReleaseShot()
    {
        //Check if the player is still shooting when button released
        if(isShooting == true) {
            isShooting = false;

            DestroyShotMeter();

            //Check if made shot and shoot the ball
            bool madeShot = player.CalculateMadeShot();
            GetComponentInChildren<BallScript>().Shoot(player.GetGoal(), madeShot);
        }
    }

    public void StartJumping()
    {
        if(isJumping == false) {
            isJumping = true;
            hands.SetHandsUp();
            player.SetSprite(PlayerScript.SpriteType.JUMPING);
            physics.HandleJumpStart();
            
        }
    }

    public void StartDunking()
    {
        isDunking = true;
        hands.SetHandsDunking();
        player.SetSprite(PlayerScript.SpriteType.DUNKING);
        physics.HandleDunkStart();
    }

    public void StartShooting()
    {
        isShooting = true;
        StartJumping();
        CreateShotMeter();
    }

    public void StopJumping()
    {
        isJumping = false;
        hands.SetHandsDown();
        player.SetSprite(PlayerScript.SpriteType.DEFAULT);
    }
    
    public void StopDunking()
    {
        isDunking = false;
        hands.SetHandsDown();
        player.SetSprite(PlayerScript.SpriteType.DEFAULT);

    }

    public void HandleWalk()
    {
        isShooting = false;
        Debug.Log("walked");

        DestroyShotMeter();
    }

    public void StartPassing()
    {
        //Set the sprite type
        player.SetSprite(PlayerScript.SpriteType.REACHING);

        //Get a reference to the target
        PlayerScript target = player.GetTeammate();

        //Handle orientation
        FaceTarget(target.transform);

        //Notify the ball
        gameLogic.GetBasketball().Pass(target);
    }

    public void AttemptSteal()
    {
        //Set the sprite type
        player.SetSprite(PlayerScript.SpriteType.REACHING);

        //Check if near the ball handler
        Transform ballHandler = physics.CheckDefenderProximity("steal");

        //Only steal if near the ball handler
        if (ballHandler != null) {
            //Do nothing if the defender fouled
            if (player.CheckForStealFoul() == true) return;

            //If probability calculated for a steal, steal ball
            if (Random.value <= attributes.GetStealProbability()) {
                gameLogic.GetBasketball().StealBall(player);
            }
        }
    }

    public void StartSprinting()
    {
        if(endurance.HasEndurance()) {
            endurance.StartMeter();
            isSprinting = true;
            player.HandleSprintSpeed(isSprinting);
        }
    }

    public void StopSprinting()
    {
        endurance.StopMeter();
        isSprinting = false;
        player.HandleSprintSpeed(isSprinting);
    }

    //Instantiate the shot meter and get a reference to the script
    public void CreateShotMeter()
    {
        ShotMeter shotMeter = player.InstantiateShotMeter();
        shotMeter.transform.parent.SetParent(transform.Find("MeterContainer"));

        endurance.gameObject.SetActive(false);
        shotMeter.transform.parent.gameObject.SetActive(true);

        gameLogic.SetPlaybackSpeed(.3f, true);
    }

    //Destroy the shotMeter after the player shoots
    public void DestroyShotMeter()
    {
        Destroy(player.GetShotMeter().transform.parent.gameObject);
        endurance.gameObject.SetActive(true);

        gameLogic.SetPlaybackSpeed(1f, false);
    }

    //Makes sure the player faces the target when shooting or passing
    private void FaceTarget(Transform target)
    {
        bool faceRight = true;

        //If the target is a goal
        if (target.GetComponent<GoalScript>() != null) {
            GoalScript goal = player.GetGoal();

            //Players goal is on the right
            if (goal.isRightGoal == true) {
                //Player is left of goal, face right
                if (transform.position.x < goal.basketCenter.position.x) {
                    faceRight = true;
                }
                //Player is right of goal, face left
                else {
                    faceRight = false;
                }
            }
            //Players goal is on the left
            else {
                //Player is left of goal, face right
                if (transform.position.x < goal.basketCenter.position.x) {
                    faceRight = true;
                }
                //Player is right of goal, face left
                else {
                    faceRight = false;
                }
            }
        }
        //Else if the target is another player
        else if (target.GetComponent<PlayerScript>() != null) {
            //If the player is left of the target, face right
            if (transform.position.x < target.position.x) {
                faceRight = true;
            }
            //If the player is right of the target, face left
            else {
                faceRight = false;
            }
        }

        //Actually apply the orientation
        if (faceRight == true) player.HandleOrientation(1);
        else player.HandleOrientation(-1);
    }

    public bool IsSprinting() { return isSprinting; }

    public bool IsShooting() { return isShooting; }

    public bool IsJumping() { return isJumping; }

    public void SetFrozen(bool newIsFrozen) { isFrozen = newIsFrozen; }

    public bool IsFrozen() { return isFrozen; }
}
