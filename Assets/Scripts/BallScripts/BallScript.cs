using System.Collections;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    public enum BallState { POSESSED, LOOSE, ON_GROUND, SHOOTING, PASSING }

    private BallState state;
    public BallState State => state;

    [SerializeField] private BallPhysicsAttributesSO fields;
    public BallPhysicsAttributesSO Fields => fields;

    [SerializeField] private float BLOCK_COOLDOWN = 2;

    private PassingPhysics passing;
    private ShootingPhysics shooting;
    private LooseBallPhysics looseBall;

    private PlayerScript currentHandler;
    private Transform looseBallContainer;
    private bool ignoringBall;

    private SpriteRenderer sprite;
    private GameObject shadowPrefab;

    private void Awake()
    {
        state = BallState.POSESSED;
        ignoringBall = false;
    }

    private void Start()
    {
        passing = new PassingPhysics(this);
        shooting = new ShootingPhysics(this);
        looseBall = new LooseBallPhysics(this);

        looseBallContainer = GameObject.Find("LooseBallContainer").transform;
        sprite = GetComponent<SpriteRenderer>();

        currentHandler = GetComponentInParent<PlayerScript>();

        shadowPrefab = Resources.Load("Prefabs/BallShadowPrefab") as GameObject;

        BallEvents.events.onBallShot += ShootEvent;
        BallEvents.events.onBallPassed += PassEvent;
        BallEvents.events.onBallTouched += BallTouchedEvent;
        BallEvents.events.onBallStolen += StealEvent;
    }

    private void FixedUpdate()
    {
        switch (State)
        {
            case BallState.PASSING:
                passing.Update();
                break;
            case BallState.SHOOTING:
                shooting.Update();
                break;
            case BallState.LOOSE:
                looseBall.Update();
                break;
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
                OnPassReceived(player);
                break;
            case BallState.LOOSE:
                looseBall.CheckForRebound(player);
                break;
            case BallState.ON_GROUND:
                OnBallPickup(player);
                break;
            case BallState.SHOOTING:
                if (player == currentHandler) return;
                shooting.CheckForBlock(currentHandler, player);
                break;
        }
    }
    
    private void ShootEvent(GoalScript goal, bool madeShot)
    {
        shooting.StartShot(currentHandler, goal, madeShot);
        state = BallState.SHOOTING;

        transform.SetParent(looseBallContainer);
    }

    private void OnPassReceived(PlayerScript player)
    {
        GameEvents.events.PassReceived(player);
        ChangePossession(player);
    }

    public void OnBallPickup(PlayerScript player)
    {
        ChangePossession(player);
    }

    public void OnShotBlocked()
    {
        looseBall.BounceOffBlock(shooting.TargetGoal, currentHandler.FrontPoint.FloorPosition);
        StartCoroutine(BlockedShotCooldown());
        state = BallState.LOOSE;

        looseBall.DrawShadow(shadowPrefab, sprite.size.y);
    }

    private IEnumerator PassWhenTargetGrounded(PlayerScript target)
    {
        PlayerStateScript playerStates = target.GetComponent<PlayerStateScript>();

        while (playerStates.IsAirborn)
        {
            yield return new WaitForSeconds(0.1f);
        }

        passing.StartPass(target);
        state = BallState.PASSING;

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
        state = BallState.LOOSE;
        looseBall.DrawShadow(shadowPrefab, sprite.size.y);
    }

    public void CompleteBouncing()
    {
        state = BallState.ON_GROUND;
    }

    public void BounceOffGoal(GoalScript targetGoal, Vector2 shooterPos, float shotSpeed)
    {
        looseBall.BounceOffGoal(targetGoal, shooterPos, shotSpeed);
    }

    public void DropFromGoal(GoalScript targetGoal) 
    { 
        looseBall.DropFromGoal(targetGoal); 
    }

    public PlayerScript GetBallHandler() { return currentHandler; }

    private void ChangePossession(PlayerScript newBallHandler)
    {
        currentHandler = newBallHandler;
        state = BallState.POSESSED;

        transform.SetParent(currentHandler.Hands);
        transform.position = currentHandler.Hands.position;

        GameEvents.events.PossessionChange(newBallHandler);
    }
}
