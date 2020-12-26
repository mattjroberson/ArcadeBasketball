using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsScript : MonoBehaviour
{
    public enum MoveState { RUNNING, JUMPING, DUNKING };
    private MoveState currentMoveState;

    private PlayerScript player;
    private ActionsScript actions;

    private RunningMovement runningMovement;
    private JumpMovement jumpMovement;
    private DunkMovement dunkMovement;

    private Transform dunkTarget;

    private ContactFilter2D groundContactFilter;
    private ContactFilter2D defenderContactFilter;
    private ContactFilter2D ballContactFilter;

    private bool movingDunkCheck;
    private bool autoDunkCheck;

    public void Awake()
    {
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

        runningMovement = new RunningMovement(this, player, actions);
        jumpMovement = new JumpMovement(this, player, actions);
        dunkMovement = new DunkMovement(this, player, actions);
    }

    public void Start()
    {
        player = GetComponent<PlayerScript>();
        actions = GetComponent<ActionsScript>();

        actions.events.onJumpBegin += jumpMovement.Start;
        actions.events.onDunkBegin += dunkMovement.Start;
        
        actions.events.onPassBegin += OnPassStartEvent;
        actions.events.onSwipeBegin += OnSwipeStartEvent;
        actions.events.onShotInit += OnShotInitEvent;
    }

    public void UpdatePhysics()
    {
        UpdatePlayerPosition();
    }

    public void SetCurrentMoveState(MoveState state) { currentMoveState = state; }

    //Handle the different movement type of the player
    private void UpdatePlayerPosition()
    {
        if (player.IsFrozen()) return;

        switch (currentMoveState) {
            case MoveState.RUNNING:
                runningMovement.Update();
                break;
            case MoveState.JUMPING:
                jumpMovement.Update();
                break;
            case MoveState.DUNKING:
                dunkMovement.Update();
                break;
        }
    }

    private void OnShotInitEvent()
    {
        FaceTarget(player.GetGoal().transform);

        if (ShouldDunk())
        {
            actions.GetDunkAction().Start();
        }
        else
        {
            actions.GetShootAction().Start();
        }
    }

    private bool ShouldDunk() { return autoDunkCheck || movingDunkCheck; }

    private void OnPassStartEvent(Transform target)
    {
        FaceTarget(target.transform);
    }

    private void OnSwipeStartEvent()
    {
        Transform ballHandler = CheckDefenderProximity("steal");

        //Only attempt steal if near the ball handler
        if (ballHandler != null) { actions.AttemptSteal(); }
    }

    //Keep these objects from moving when the player jumps
    public void HandleLockedElements(bool attach)
    {
        if (attach) {
            player.GetFrontPoint().SetParent(transform);
        }
        else {
            player.GetFrontPoint().SetParent(null);
        }
    }

    //Makes sure the player faces the target when shooting or passing
    private void FaceTarget(Transform target)
    {
        bool faceRight = true;

        //If the target is a goal
        if (target.GetComponent<GoalScript>() != null)
        {
            GoalScript goal = player.GetGoal();

            //Players goal is on the right
            if (goal.isRightGoal == true)
            {
                //Player is left of goal, face right
                if (transform.position.x < goal.basketCenter.position.x)
                {
                    faceRight = true;
                }
                //Player is right of goal, face left
                else
                {
                    faceRight = false;
                }
            }
            //Players goal is on the left
            else
            {
                //Player is left of goal, face right
                if (transform.position.x < goal.basketCenter.position.x)
                {
                    faceRight = true;
                }
                //Player is right of goal, face left
                else
                {
                    faceRight = false;
                }
            }
        }
        //Else if the target is another player
        else if (target.GetComponent<PlayerScript>() != null)
        {
            //If the player is left of the target, face right
            if (transform.position.x < target.position.x)
            {
                faceRight = true;
            }
            //If the player is right of the target, face left
            else
            {
                faceRight = false;
            }
        }

        //Actually apply the orientation
        if (faceRight == true) HandleOrientation(1);
        else HandleOrientation(-1);
    }

    //Makes sure the player is facing the correct way
    private void HandleOrientation(float xMovement)
    {
        float rotation = transform.rotation.y;

        bool needsFlip = (rotation == 0) ? (xMovement < 0) : (xMovement > 0);

        if (needsFlip)
        {
            rotation = (rotation == 0) ? 180f : 0f;
            transform.rotation = Quaternion.AngleAxis(rotation, Vector3.up);

            //Set a variable to track it other places
            player.SetFacingRight(transform.rotation.y == 0 ? true : false);
        }
    }

    //Raycasts to see if their is a player nearby by for stealing and shot contests.
    private Transform CheckDefenderProximity(string type)
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "shotZone") {
            player.SetShotZoneName(other.name);
        }
        else if(other.tag == "dunkZone") {
            SetDunkZone(other.name, true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "dunkZone") {
            SetDunkZone(other.name, false);
        }
    }

    //Handle the boolean values for the zones the player could dunk at
    private void SetDunkZone(string zoneName, bool enteringZone)
    {
        switch (zoneName) {
            case "moving_dunk":
                movingDunkCheck = enteringZone;
                break;
            case "auto_dunk":
                autoDunkCheck = enteringZone;
                break;
        }
    }
}
