using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsScript : MonoBehaviour
{
    public enum MoveState { RUNNING, JUMPING, DUNKING };
    private MoveState currentMoveState;

    private PlayerScript player;
    private IntelligenceContainer intelligence;
    private ActionsScript actions;
    private AttributeScript attributes;

    private BallScript basketball;

    private Vector2 currentVelocity;
    private Vector2 targetVelocity;

    private BoxCollider2D footCollider;
    private BoxCollider2D handCollider;

    private Transform dunkTarget;

    private ContactFilter2D groundContactFilter;
    private ContactFilter2D defenderContactFilter;
    private ContactFilter2D ballContactFilter;
    private float feetHitMargin;

    private float minimumMoveDistance;
    private float minimumFlipDistance;
    private float ySpeedModifier;
    private float xSlopeVelocity;
    private float xSlopeModifier;

    private float[] dunkTrajectory;
    private float dunkSpeed;
    private float dunkAngle;
    private float dunkArcPeakPercent;

    private Vector2 jumpFloor;

    private bool movingDunkCheck;
    private bool autoDunkCheck;

    public void Awake()
    {
        targetVelocity = Vector2.zero;
        minimumMoveDistance = 0.001f;
        minimumFlipDistance = 0.02f;

        currentMoveState = MoveState.RUNNING;

        groundContactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("FootColliders")));
        groundContactFilter.useLayerMask = true;
        groundContactFilter.useTriggers = false;

        ballContactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("BallOnCourtFilter")));
        ballContactFilter.useLayerMask = true;
        ballContactFilter.useTriggers = false;

        defenderContactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("DefenderFilter")));
        defenderContactFilter.useLayerMask = true;
        defenderContactFilter.useTriggers = false;

        feetHitMargin = 0.01f;
        ySpeedModifier = 1.5f;
        xSlopeModifier = 1.5f;

        dunkSpeed = 2.25f;
        dunkAngle = Mathf.PI / 3;
        dunkArcPeakPercent = .5f;
    }

    public void Start()
    {
        player = GetComponent<PlayerScript>();
        actions = GetComponent<ActionsScript>();
        attributes = GetComponent<AttributeScript>();
        intelligence = GetComponentInChildren<IntelligenceContainer>();

        basketball = GameObject.Find("Basketball").GetComponent<BallScript>();

        footCollider = transform.Find("ColliderContainer").Find("FootCollider").GetComponent<BoxCollider2D>();
        handCollider = transform.Find("ColliderContainer").Find("HandCollider").GetComponent<BoxCollider2D>();

        actions.events.onJumpBegin += OnJumpStartEvent;
        actions.events.onDunkBegin += OnDunkStartEvent;
    }

    public void UpdatePhysics()
    {
        targetVelocity = ComputeTargetVelocity();
        UpdatePlayerPosition();
    }

    public void SetMoveState(MoveState newState) { currentMoveState = newState; }

    //Get the velocity as the players direction scaled by its speed
    private Vector2 ComputeTargetVelocity()
    {
        return intelligence.Current().GetMoveDirection() * player.GetCurrentSpeed();
    }

    //Handle the different movement type of the player
    private void UpdatePlayerPosition()
    {
        if (actions.IsFrozen()) return;

        switch (currentMoveState) {
            case MoveState.RUNNING:
                UpdateRunning();
                break;
            case MoveState.JUMPING:
                UpdateJumping();
                break;
            case MoveState.DUNKING:
                UpdateDunking();
                break;
        }
    }

    //Update the movement when the player is on the ground
    private void UpdateRunning()
    {
        currentVelocity.x = (targetVelocity.x == 0) ? xSlopeVelocity : targetVelocity.x;
        currentVelocity.y = targetVelocity.y / ySpeedModifier;

        Vector2 delta_position = currentVelocity * Time.deltaTime;

        Vector2 move = Vector2.right * delta_position.x;
        ApplyRunningMovement(move, false);

        move = Vector2.up * delta_position.y;
        ApplyRunningMovement(move, true);
    }

    //Handle the beginning of a jump
    public void OnJumpStartEvent()
    {
        currentMoveState = MoveState.JUMPING;

        jumpFloor = transform.position;
        HandleLockedElements(false);

        currentVelocity = new Vector2(0, attributes.GetMaxJump());
    }

    //Update the movement when the player is jumping
    private void UpdateJumping()
    {
        //Actually update the position
        currentVelocity += Physics2D.gravity * Time.deltaTime;

        Vector2 movement = currentVelocity * Time.deltaTime;
        transform.position = transform.position + (Vector3)movement;

        //If player back on ground, stop jumping
        if (transform.position.y <= jumpFloor.y) {
            HandleJumpEnd();
        }
    }

    private void HandleJumpEnd()
    {
        transform.position = jumpFloor;
        HandleLockedElements(true);

        currentMoveState = MoveState.RUNNING;
        actions.GetJumpAction().Stop();
    }

    //Handle the beginning of the dunk
    public void OnDunkStartEvent()
    {
        currentMoveState = MoveState.DUNKING;

        dunkTrajectory = TrajectoryScript.CalculateTrajectory(transform.position, new Vector2(5.3f, .2f), dunkAngle, dunkArcPeakPercent);
        HandleLockedElements(false);

        currentVelocity = Vector2.zero;
    }

    //Update the movement when the player is dunking
    private void UpdateDunking()
    {
        float newX = transform.position.x + (dunkSpeed * Time.deltaTime);
        float newY = (dunkTrajectory[0] * newX * newX) + (dunkTrajectory[1] * newX) + dunkTrajectory[2];

        transform.position = new Vector2(newX, newY);

        //If player reaches the dunk target, stop dunking
        if (transform.position.x >= 5.3f && transform.position.y <= .2f) {
            HandleDunkEnd();
        }
    }

    //TODO Fix  Hardcode value
    private void HandleDunkEnd()
    {
        currentMoveState = MoveState.JUMPING;
        jumpFloor = new Vector2(transform.position.x, -.8f);
        actions.GetDunkAction().Stop();
    }

    //Keep these objects from moving when the player jumps
    private void HandleLockedElements(bool attach)
    {
        if (attach) {
            //footCollider.transform.SetParent(transform.Find("ColliderContainer"));
            player.GetFrontPoint().SetParent(transform);
        }
        else {
            //footCollider.transform.SetParent(null);
            player.GetFrontPoint().SetParent(null);
        }
    }
    
    //Actually apply the movement to the player, to each axis individually
    private void ApplyRunningMovement(Vector2 newMove, bool isYMovement)
    {
        Vector2 direction = newMove.normalized;
        float distance = newMove.magnitude;

        RaycastHit2D[] collisionArray = new RaycastHit2D[16];

        //If player moved enough to register
        if (distance > minimumMoveDistance) {
            //player.AddDistanceMoved(distance);

            //Get a count of how many things the player will collide with in this movement
            int collisionCount = footCollider.Cast(direction, groundContactFilter, collisionArray, distance + feetHitMargin);

            for (int i = 0; i < collisionCount; i++) {
                Vector2 currentNormal = collisionArray[i].normal;

                //Allows sliding along slopes
                if (isYMovement && currentNormal.y != 1 && currentNormal.y != -1) {
                    float yNormal = currentNormal.y;

                    //If player is on the left side, flip the y normal
                    if (transform.position.x < 0) {
                        yNormal *= -1;
                    }

                    Vector2 velocityAlongSlope = new Vector2(yNormal * xSlopeModifier, currentNormal.x);

                    currentVelocity = velocityAlongSlope;
                    xSlopeVelocity = currentVelocity.x;
                }


                float modifiedDistance = collisionArray[i].distance - feetHitMargin;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }

            //If nothing was collided with, make slopeVelocity 0
            if (collisionCount == 0) {
                xSlopeVelocity = 0;
            }

            Vector2 adjustedMovement = newMove.normalized * distance;

            //Update the direction the player is facing. If movement is less than minimum, pass 0
            float xMovement = (Mathf.Abs(adjustedMovement.x) > minimumFlipDistance) ? adjustedMovement.x : 0;
            player.HandleOrientation(xMovement);

            //Update the position of the player
            transform.position = transform.position + (Vector3)adjustedMovement;

            //Check if the player ran over a ball on the court
            CheckBasketballGrab(adjustedMovement);
        }
    }

    //Raycasts to see if their is a player nearby by for stealing and shot contests.
    public Transform CheckDefenderProximity(string type)
    {
        //Adopt orientation of the player
        Vector2 rayDirection;
        rayDirection = player.GetFacingRight() ? Vector2.right : Vector2.left;

        //Start Raycast from the front of the player
        Vector2 raySource = player.GetFrontPoint().position;

        //Cast three rays
        int numRays = 3;

        float rayDistance = 0f;
        float rayPosY = 0f;
        float rayGapY = 0f;

        Vector2 rayRotation = Vector2.zero;

        //Set Variables dependant on type
        if (type.Equals("steal")) {
            rayDistance = .75f;
            rayPosY = -.075f;
            rayGapY = .075f;
        }
        else if (type.Equals("shoot")) {
            //Calculate the direction between the goal and the shooter
            rayRotation = (player.GetGoal().baseOfGoal.position - (Vector3)raySource).normalized;

            rayDistance = 1.1f;
            rayPosY = -10f;
            rayGapY = 10f;
        }

        //Cast every ray
        for (int i = 0; i < numRays; i++) {

            //Steal checks straight ahead, while shoot checks rotated toward the goal
            if (type.Equals("steal")) {
                rayDirection = new Vector2(rayDirection.x, rayPosY);
            }
            else if (type.Equals("shoot")) {
                rayDirection = Quaternion.Euler(0, 0, rayPosY) * rayRotation;
            }

            //Actually cast the ray and store the result
            Transform ray = ProjectRaycast(raySource, rayDirection, rayDistance, type, true);

            //If the ray hit something, return it
            if (ray != null) return ray;

            //Adjust the y position of the next ray
            rayPosY += rayGapY;
        }

        //If nothing was detected, return null
        return null;
    }

    //Creates a raycast to check for player proximity
    private Transform ProjectRaycast(Vector2 location, Vector2 direction, float distance, string type, bool debug)
    {
        RaycastHit2D[] collisions = new RaycastHit2D[16];

        //Create the raycast
        int count = Physics2D.Raycast(location, direction, defenderContactFilter, collisions, distance);

        //Draw the rays if in debug mode
        if (debug) Debug.DrawLine(location, location + (direction * distance), Color.red, 5);

        for (int i = 0; i < count; i++) {
            //For steal logic
            if (type.Equals("steal")) {
                //Get a refence to the player detected
                PlayerScript testPlayer = collisions[i].transform.GetComponent<PlayerScript>();

                //Only return the player if they have the ball
                if (testPlayer.GetHasBall() == true) {
                    return collisions[i].transform;
                }
            }
            //For shoot logic
            else if (type.Equals("shoot")) {
                //Only return the player if they are on different teams
                if (collisions[i].transform.parent != player.transform.parent) {
                    return collisions[i].transform;
                }
            }
        }

        //If nothing detected return null
        return null;
    }

    private void CheckBasketballGrab(Vector2 move)
    {
        if (basketball.GetBallState() == BallScript.BallState.ON_GROUND) {

            RaycastHit2D[] collisionArray = new RaycastHit2D[16];

            int count = footCollider.Cast(move, ballContactFilter, collisionArray, move.magnitude);

            if (count > 0) {
                basketball.GrabBall(transform.GetComponent<PlayerScript>());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "shotZone") {
            player.SetShotZoneName(other.name);
        }
        else if(other.tag == "dunkZone") {
            HandleDunkZone(other.name, true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "dunkZone") {
            HandleDunkZone(other.name, false);
        }
    }

    //Handle the boolean values for the zones the player could dunk at
    private void HandleDunkZone(string zoneName, bool enteringZone)
    {
        //isEnter = true when in zone, false when not

        switch (zoneName) {
            case "moving_dunk":
                movingDunkCheck = enteringZone;
                break;
            case "auto_dunk":
                autoDunkCheck = enteringZone;
                break;
        }
    }

    public bool ShouldDunk() { return autoDunkCheck || movingDunkCheck;  }
}
