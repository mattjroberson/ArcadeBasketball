using UnityEngine;

public class IsoPhysics : MonoBehaviour
{
    public enum MoveState { RUNNING, JUMPING, DUNKING };
    private MoveState currentState;

    private PlayerScript player;
    private ActionsScript actions;
    private PlayerStateScript playerStates;
    private Rigidbody2D rb;

    private float currentSpeed;

    //Jumping Variables
    private CapsuleCollider2D feetCollider;
    private Vector2 jumpVelocity;
    private float floorY;
    private const float FLOOR_MARGIN = .01f;

    //Dunking Variables
    private float[] dunkTrajectory;
    private readonly Vector2 DUNK_TARGET = new Vector2(5.3f, .2f);
    private const float DUNK_ANGLE = Mathf.PI / 3;
    private const float DUNK_ARC_PEAK_PERCENT = .5f;
    private const float DUNK_X_SPEED = 2.25f;
    private const float DUNK_FLOOR = -.8f;

    public void Start()
    {
        player = GetComponent<PlayerScript>();
        actions = GetComponent<ActionsScript>();
        playerStates = GetComponent<PlayerStateScript>();
        feetCollider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        currentSpeed = player.GetAttributes().GetMaxSpeed();
        currentState = MoveState.RUNNING;

        actions.events.onJumpBegin += JumpBeginEvent;
        actions.events.onDunkBegin += DunkBeginEvent;
        actions.events.onShotInit += ShootBeginEvent;
        actions.events.onPassBegin += PassBeginEvent;
        actions.events.onSprintBegin += SprintBeginEvent;
        actions.events.onSprintEnd += SprintEndEvent;
    }

    public void FixedUpdate()
    {
        switch (currentState)
        {
            case MoveState.RUNNING:
                RunningUpdate();
                break;
            case MoveState.JUMPING:
                JumpingUpdate();
                break;
            case MoveState.DUNKING:
                DunkingUpdate();
                break;
        }
    }

    private void RunningUpdate()
    {
        if (playerStates.IsFrozen()) return;

        Vector2 currentPos = rb.position;

        Vector2 inputVector = playerStates.GetCurrentMoveDirection();
        inputVector = Vector2.ClampMagnitude(inputVector, 1);

        Vector2 movement = inputVector * currentSpeed;
        Vector2 newPos = currentPos + movement * Time.fixedDeltaTime;

        HandleOrientation(movement.x);
        rb.MovePosition(newPos);
    }

    private void JumpBeginEvent()
    {
        jumpVelocity = new Vector2(0, player.GetAttributes().GetMaxJump());
        floorY = rb.position.y;

        feetCollider.enabled = false;

        currentState = MoveState.JUMPING;
    }

    private void JumpingUpdate()
    {
        jumpVelocity += Physics2D.gravity * Time.deltaTime;

        Vector2 distMoved = jumpVelocity * Time.deltaTime;
        rb.MovePosition(rb.position + distMoved);

        if (rb.position.y <= (floorY - FLOOR_MARGIN)) JumpingStop();
    }

    private void JumpingStop()
    {
        rb.MovePosition(new Vector2(rb.position.x, floorY));

        feetCollider.enabled = true;

        currentState = MoveState.RUNNING;
        actions.GetJumpAction().Stop();
    }

    private void DunkBeginEvent()
    {  
        dunkTrajectory = TrajectoryScript.CalculateTrajectory(rb.position, DUNK_TARGET, DUNK_ANGLE, DUNK_ARC_PEAK_PERCENT);

        feetCollider.enabled = false;
        currentState = MoveState.DUNKING;
    }

    private void DunkingUpdate()
    {
        float newX = rb.position.x + (DUNK_X_SPEED * Time.deltaTime);
        float newY = (dunkTrajectory[0] * newX * newX) + (dunkTrajectory[1] * newX) + dunkTrajectory[2];

        rb.MovePosition(new Vector2(newX, newY));

        //If player reaches the dunk target, stop dunking
        if (rb.position.x >= DUNK_TARGET.x && rb.position.y <= DUNK_TARGET.y)
        {
            DunkingStop();
        }
    }

    private void DunkingStop()
    {
        floorY = DUNK_FLOOR;

        currentState = MoveState.JUMPING;
        actions.GetDunkAction().Stop();
    }
    
    private void ShootBeginEvent()
    {
        FaceGoal();
    }

    private void PassBeginEvent(Transform target)
    {
        FacePlayer(target);
    }

    private void SprintBeginEvent()
    {
        currentSpeed *= player.GetAttributes().GetSprintBonus();
    }

    private void SprintEndEvent()
    {
        currentSpeed = player.GetAttributes().GetMaxSpeed();
    }

    private void FaceGoal()
    {
        GoalScript goal = player.GetGoal();

        bool faceRight = (transform.position.x < goal.basketCenter.position.x) ? true : false;
        if (goal.isRightGoal == false) faceRight = !faceRight;

        HandleOrientation(faceRight ? 1 : -1);
    }

    private void FacePlayer(Transform target)
    {
        bool faceRight = transform.position.x < target.position.x ? true : false;
        HandleOrientation(faceRight ? 1 : -1);
    }

    private void HandleOrientation(float xMovement)
    {
        float rotation = transform.rotation.y;
        bool needsFlip = (rotation == 0) ? (xMovement < 0) : (xMovement > 0);

        if (needsFlip)
        {
            rotation = (rotation == 0) ? 180f : 0f;
            transform.rotation = Quaternion.AngleAxis(rotation, Vector3.up);
        }
    }
}
