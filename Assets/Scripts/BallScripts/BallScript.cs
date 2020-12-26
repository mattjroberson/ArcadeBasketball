using System.Collections;
using System.Collections.Generic;
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
    private PlayerScript targetPlayer;
    private GameLogicScript gameLogic;

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
        gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogicScript>();
        physics = GetComponent<BallPhysicsScript>();
        sprite = GetComponent<SpriteRenderer>();

        //Get a reference to the current ball handler
        currentPlayer = transform.GetComponentInParent<PlayerScript>();

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
        //Update the state of the ball
        SetBallState(BallState.POSESSED);

        //Attach the ball to the player and update game logic, unfreeze player
        transform.SetParent(target.GetHands());
        currentPlayer = target;

        gameLogic.UpdatePossession(target);
        target.SetFrozen(false);
    }

    public void StealPass(PlayerScript defender, PlayerScript target)
    {
        //Update the state of the ball
        SetBallState(BallState.POSESSED);

        //Attach the ball to the player and update game logic, unfreeze target player
        currentPlayer = defender;
        transform.SetParent(defender.GetHands());

        gameLogic.UpdatePossession(defender);
        target.SetFrozen(false);
    }

    private void StealEvent(PlayerScript defender)
    {
        //Attach the ball to the defender and clear its local position
        currentPlayer = defender;
        transform.SetParent(defender.GetHands());
        physics.SetLocalPosition(Vector2.zero);

        //Update the game logic
        gameLogic.UpdatePossession(defender);
    }

    private void LooseBallPickupEvent(PlayerScript player)
    {
        GrabBall(player);
    }

    private IEnumerator PassWhenTargetGrounded(PlayerScript target)
    {
        ActionsScript currentActionsScript = target.GetComponent<ActionsScript>();
        targetPlayer = target;

        //Wait till the player is on the ground to pass
        while (currentActionsScript.GetJumpAction().IsActive()) {
            yield return new WaitForSeconds(0.1f);
        }

        target.SetFrozen(true);

        //Handle the physics of the pass
        physics.StartPass(target);
        SetBallState(BallState.PASSING);

        //Dettach the ball from the player and notify the player
        currentPlayer.SetHasBall(false);
        transform.SetParent(looseBallContainer);
    }

    private void ShootEvent(GoalScript goal, bool madeShot)
    {
        this.madeShot = madeShot;

        physics.StartShot(goal);
        SetBallState(BallState.SHOOTING);

        //Dettach the ball from the player and notify the player
        currentPlayer.SetHasBall(false);
        transform.SetParent(looseBallContainer);
    }

    public void HandleShotFinished()
    {
        SetBallState(BallState.LOOSE);
        DrawShadow();
    }

    public void GrabBall(PlayerScript player)
    {
        //Update the state of the ball
        SetBallState(BallState.POSESSED);

        //Attach the ball to the player and update game logic
        currentPlayer = player;
        transform.SetParent(player.GetHands());
        physics.SetPosition(player.GetHands().position);

        gameLogic.UpdatePossession(player);
    }

    public void BlockShot()
    {
        Debug.Log("shot blocked");
        SetBallState(BallState.LOOSE);
        SetBallFloor(currentPlayer.GetFrontPoint().position);
        DrawShadow();

        physics.StartBlock();
    }

    public void DrawShadow()
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

    public PlayerScript GetTargetPlayer() { return targetPlayer; }

    public bool GetMadeShot() { return madeShot; }

    public Vector2 GetFloor() { return ballFloor; }

}
