using System.Collections;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    public enum BallState { POSESSED, LOOSE, ON_GROUND, SHOOTING, PASSING }
    private static BallState ballState;

    private GameObject shadowPrefab;

    private Transform looseBallContainer;
    private BallPhysicsScript physics;
    private SpriteRenderer sprite;

    private PlayerScript currentPlayer;

    private Vector2 madeShotBallFloor;
    private Vector2 ballFloor;

    private bool madeShot;

    public void Awake()
    {
        SetBallState(BallState.POSESSED);
    }

    public void Start()
    {
        looseBallContainer = GameObject.Find("LooseBallContainer").transform;
        physics = GetComponent<BallPhysicsScript>();
        sprite = GetComponent<SpriteRenderer>();

        shadowPrefab = Resources.Load("Prefabs/BallShadowPrefab") as GameObject;

        GameEvents.events.onBallShot += ShootEvent;
        GameEvents.events.onBallPassed += PassEvent;
        GameEvents.events.onBallStolen += StealEvent;
        GameEvents.events.onLooseBallPickup += LooseBallPickupEvent;
    }

    public void LateUpdate()
    {
        //Only apply physics if not in a players hands
        if(ballState != BallState.POSESSED) {
            physics.UpdatePhysics();
        }
    }

    private void PassEvent(PlayerScript target)
    {
        StartCoroutine(PassWhenTargetGrounded(target));
    }

    public void RecievePass(PlayerScript target)
    {
        ChangePossession(target);
        GameEvents.events.PassReceived(target);
    }

    public void StealPass(PlayerScript defender, PlayerScript target)
    {
        ChangePossession(defender);
        GameEvents.events.PassStolen(target);
    }

    private void StealEvent(PlayerScript defender)
    {
        ChangePossession(defender);

        //clear its local position
        physics.SetLocalPosition(Vector2.zero);

    }

    private void LooseBallPickupEvent(PlayerScript player)
    {
        GrabBall(player);
    }

    private IEnumerator PassWhenTargetGrounded(PlayerScript target)
    {
        ActionsScript currentActionsScript = target.GetComponent<ActionsScript>();

        //Wait till the player is on the ground to pass
        while (currentActionsScript.GetJumpAction().IsActive()) {
            yield return new WaitForSeconds(0.1f);
        }

        //Handle the physics of the pass
        physics.StartPass(target);
        SetBallState(BallState.PASSING);

        //Dettach the ball from the player and notify the player
        transform.SetParent(looseBallContainer);
    }

    private void ShootEvent(GoalScript goal, bool madeShot)
    {
        this.madeShot = madeShot;

        physics.StartShot(goal);
        SetBallState(BallState.SHOOTING);

        transform.SetParent(looseBallContainer);
    }

    public void HandleShotFinished()
    {
        SetBallState(BallState.LOOSE);
        DrawShadow();
    }

    public void GrabBall(PlayerScript player)
    {
        ChangePossession(player);
    }

    public void BlockShot()
    {
        SetBallState(BallState.LOOSE);
        SetBallFloor(currentPlayer.FrontPoint.position);
        DrawShadow();

        physics.StartBlock();
    }

    private void ChangePossession(PlayerScript newBallHandler)
    {
        currentPlayer = newBallHandler;
        ballState = BallState.POSESSED;

        transform.SetParent(currentPlayer.Hands);
        physics.SetPosition(currentPlayer.Hands.position);

        GameEvents.events.PossessionChange(newBallHandler);
    }

    private void DrawShadow()
    {
        float timeTillGround = physics.CalculateTimeTillGround();

        Vector2 shadowPos = new Vector2();
        shadowPos.x = physics.CalculateXPositionAtTime(timeTillGround);
        shadowPos.y = ballFloor.y - (sprite.size.y / 2);

        GameObject shadow = Instantiate(shadowPrefab, shadowPos, Quaternion.identity);
        shadow.GetComponent<BallShadowScript>().Focus(timeTillGround);
    }

    public static BallState GetBallState() { return ballState; }

    public static void SetBallState(BallState newBallState) { ballState = newBallState; }

    public void SetBallFloor(Vector2 target) { ballFloor = target; }

    public Vector2 GetTargetPosition() { return physics.GetTarget(); }

    public PlayerScript GetCurrentPlayer() { return currentPlayer; }

    public bool GetMadeShot() { return madeShot; }

    public Vector2 GetFloor() { return ballFloor; }

}
