using System.Collections;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    public enum BallState { POSESSED, LOOSE, ON_GROUND, SHOOTING, PASSING }
    public BallState State { get; set; }

    [SerializeField] private const float BLOCK_COOLDOWN = 2;

    public PlayerScript CurrentHandler { get; set; }
    public Vector2 BallFloor { get; set; }
   
    private GameObject shadowPrefab;

    private Transform looseBallContainer;
    private BallPhysicsScript physics;
    private SpriteRenderer sprite;

    private bool ignoringBall;

    public void Awake()
    {
        State = BallState.POSESSED;
        ignoringBall = false;
    }

    public void Start()
    {
        looseBallContainer = GameObject.Find("LooseBallContainer").transform;
        physics = GetComponent<BallPhysicsScript>();
        sprite = GetComponent<SpriteRenderer>();

        CurrentHandler = GetComponentInParent<PlayerScript>();

        shadowPrefab = Resources.Load("Prefabs/BallShadowPrefab") as GameObject;

        BallEvents.events.onBallShot += ShootEvent;
        BallEvents.events.onBallPassed += PassEvent;
        BallEvents.events.onBallTouched += BallTouchedEvent;
        BallEvents.events.onBallStolen += StealEvent;
    }

    public void FixedUpdate()
    {
        //Only apply physics if not in a players hands
        if(State != BallState.POSESSED) {
            physics.UpdatePhysics();
        }
    }

    private void PassEvent(PlayerScript target)
    {
        StartCoroutine(PassWhenTargetGrounded(target));
    }

    private void StealEvent(PlayerScript defender)
    {
        ChangePossession(defender);
    }

    private void BallTouchedEvent(PlayerScript player)
    {
        if (ignoringBall) return;

        switch (State)
        {
            case BallState.PASSING:
                GameEvents.events.PassReceived(player);
                ChangePossession(player);
                break;
            case BallState.LOOSE:
                if (LooseBallPhysics.IsInReboundRange(player.FrontPoint, BallFloor)) ChangePossession(player);
                break;
            case BallState.ON_GROUND:
                ChangePossession(player);
                break;
            case BallState.SHOOTING:
                if (player == CurrentHandler) return;

                if (ShootingPhysics.IsInBlockRange(CurrentHandler, player, physics.Shooting.TargetGoal, physics.Fields))
                {
                    physics.LooseBall.BounceOffBlock();
                    StartCoroutine(BlockedShotCooldown());
                    State = BallState.LOOSE;
                    
                    DrawShadow();
                }
                break;
        }
    }

    private void ShootEvent(GoalScript goal, bool madeShot)
    {
        physics.Shooting.StartShot(goal, madeShot);
        State = BallState.SHOOTING;

        transform.SetParent(looseBallContainer);
    }

    private IEnumerator PassWhenTargetGrounded(PlayerScript target)
    {
        PlayerStateScript playerStates = target.GetComponent<PlayerStateScript>();

        //Wait till the player is on the ground to pass
        while (playerStates.IsAirborn)
        {
            yield return new WaitForSeconds(0.1f);
        }

        //Handle the physics of the pass
        physics.Passing.StartPass(target);
        State = BallState.PASSING;

        //Dettach the ball from the player and notify the player
        transform.SetParent(looseBallContainer);
        GameEvents.events.PassSent(target);
    }

    private IEnumerator BlockedShotCooldown()
    {
        ignoringBall = true;
        yield return new WaitForSeconds(BLOCK_COOLDOWN);
        ignoringBall = false;
    }

    public void FinishShot()
    {
        State = BallState.LOOSE;
        DrawShadow();
    }

    private void ChangePossession(PlayerScript newBallHandler)
    {
        CurrentHandler = newBallHandler;
        State = BallState.POSESSED;

        transform.SetParent(CurrentHandler.Hands);
        physics.SetPosition(CurrentHandler.Hands.position);

        GameEvents.events.PossessionChange(newBallHandler);
    }

    private void DrawShadow()
    {
        float timeTillGround = LooseBallPhysics.CalculateTimeTillGround(physics.Velocity.y, BallFloor, transform.position);

        Vector2 shadowPos = new Vector2();
        shadowPos.x = physics.CalculateXPositionAtTime(timeTillGround);
        shadowPos.y = BallFloor.y - (sprite.size.y / 2);

        GameObject shadow = Instantiate(shadowPrefab, shadowPos, Quaternion.identity);
        shadow.GetComponent<BallShadowScript>().Focus(timeTillGround);
    }
}
