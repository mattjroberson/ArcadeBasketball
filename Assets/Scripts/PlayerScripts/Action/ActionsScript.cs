using UnityEngine;

public class ActionsScript : MonoBehaviour
{
    private PlayerScript player;
    private PhysicsScript physics;
    private AttributeScript attributes;

    public ActionEvents events;

    //TODO Convert to super class instead of interface???
    private IAction jumpAction;
    private IAction sprintAction;
    private ShootAction shootAction;
    private IAction dunkAction;

    private bool isFrozen;

    public void Awake()
    {
        events = new ActionEvents();
    }

    public void Start()
    {
        //gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogicScript>();

        player = GetComponent<PlayerScript>();
        physics = GetComponent<PhysicsScript>();
        attributes = GetComponent<AttributeScript>();

        jumpAction = new JumpAction(this);
        sprintAction = new SprintAction(this);
        shootAction = new ShootAction(this);
        dunkAction = new DunkAction(this);

        events.onWalkViolation += shootAction.PlayerWalked;
        ;
    }

    public void InitializeShot()
    {
        FaceTarget(player.GetGoal().transform);

        if (physics.ShouldDunk()) {
            dunkAction.Start();
        }
        else {
            shootAction.Start();
        }  
    }

    public void StartPassing()
    {
        //Get a reference to the target
        PlayerScript target = player.GetTeammate();

        //Handle orientation
        FaceTarget(target.transform);

        events.PassBegin();
        GameEvents.events.BallPassed(target);
    }

    public void AttemptSteal()
    {
        events.StealBegin();

        //Check if near the ball handler
        Transform ballHandler = physics.CheckDefenderProximity("steal");

        //Only steal if near the ball handler
        if (ballHandler != null) {
            //Do nothing if the defender fouled
            if (player.CheckForStealFoul() == true) return;

            //If probability calculated for a steal, steal ball
            if (Random.value <= attributes.GetStealProbability()) {
                GameEvents.events.BallStolen(player);
            }
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
        if (faceRight == true) player.HandleOrientation(1);
        else player.HandleOrientation(-1);
    }

    public void CompleteShotProcess()
    {
        bool madeShot = player.CalculateMadeShot();
        GameEvents.events.BallShot(player.GetGoal(), madeShot);
    }

    public IAction GetJumpAction() { return jumpAction; }

    public IAction GetSprintAction() { return sprintAction; }

    public IAction GetShootAction() { return shootAction;  }

    public IAction GetDunkAction() { return dunkAction; }

    public void SetFrozen(bool newIsFrozen) { isFrozen = newIsFrozen; }

    public bool IsFrozen() { return isFrozen; }
}
