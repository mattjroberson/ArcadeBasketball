using UnityEngine;

public class MovePhysics : MonoBehaviour
{
    public enum MoveState { RUNNING, JUMPING, DUNKING };
    public MoveState CurrentState { get; set; }

    private CapsuleCollider2D feetCollider;

    private PlayerScript player;
    private ActionsScript actions;

    private RunningPhysics runningPhysics;
    private JumpingPhysics jumpingPhysics;
    private DunkingPhysics dunkingPhysics;

    private Rigidbody2D rb;
    public Rigidbody2D Rigidbody => rb;

    [Header("Dunk Fields")]

    [SerializeField] private float meanDunkLength = 1.2f;
    public float MEAN_DUNK_LENGTH => meanDunkLength;

    [SerializeField] private float meanDunkXSpeed = 2f;
    public float MEAN_DUNK_X_SPEED => meanDunkXSpeed;

    [SerializeField] private float maxDunkXSpeed = 2.2f;
    public float MAX_DUNK_X_SPEED => maxDunkXSpeed;

    [SerializeField] private float straightDunkSpeed = 4f;
    public float STRAIGHT_DUNK_SPEED => straightDunkSpeed;

    public void Start()
    {
        player = GetComponent<PlayerScript>();
        actions = GetComponent<ActionsScript>();
        feetCollider = GetComponentInChildren<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        runningPhysics = new RunningPhysics(this, player, actions);
        jumpingPhysics = new JumpingPhysics(this, player, actions);
        dunkingPhysics = new DunkingPhysics(this, player, actions);

        CurrentState = MoveState.RUNNING;

        actions.events.onShootBegin += ShootBeginEvent;
        actions.events.onPassBegin += PassBeginEvent;
    }

    public void FixedUpdate()
    {
        switch (CurrentState)
        {
            case MoveState.RUNNING:
                runningPhysics.Update();
                break;
            case MoveState.JUMPING:
                jumpingPhysics.Update();
                break;
            case MoveState.DUNKING:
                dunkingPhysics.Update();
                break;
        }
    }

    public void SetAirborn(bool airborn)
    {
        player.States.IsAirborn = airborn;
        feetCollider.enabled = !airborn;
    }

    public void SetJumpY(float jumpY)
    {
        jumpingPhysics.JumpY = jumpY;
    }

    public void HandleOrientation(float xMovement)
    {
        float rotation = transform.rotation.y;
        bool needsFlip = (rotation == 0) ? (xMovement < 0) : (xMovement > 0);

        if (needsFlip)
        {
            rotation = (rotation == 0) ? 180f : 0f;
            transform.rotation = Quaternion.AngleAxis(rotation, Vector3.up);
        }
    }

    private void ShootBeginEvent()
    {
        FaceTarget(player.Team.Side.Goal.basketCenter);
    }

    private void PassBeginEvent(Transform target)
    {
        FaceTarget(target);
    }

    private void FaceTarget(Transform target)
    {
        bool faceRight = Rigidbody.position.x < target.position.x ? true : false;
        //if (goal.isRightGoal == false) faceRight = !faceRight;

        HandleOrientation(faceRight ? 1 : -1);
    }
}
