using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseBallPhysics 
{
    private BallPhysicsScript physics;
    private ContactFilter2D rbndContactFilter;

    private float minimumBounceVelocity;
    private float ballBounceFactor;

    private float minBlockVelocityX;
    private float maxBlockVelocityX;
    private float blockVelocityY;

    public LooseBallPhysics(BallPhysicsScript physics)
    {
        this.physics = physics;

        minimumBounceVelocity = 1f;
        ballBounceFactor = .60f;

        minBlockVelocityX = .1f;
        maxBlockVelocityX = 3f;
        blockVelocityY = .1f;

        rbndContactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("ReboundFilter")));
        rbndContactFilter.useLayerMask = true;
        rbndContactFilter.useTriggers = false;
    }

    public void StartRebound()
    {
        physics.SetVelocity(CalculateReboundVelocity());
        physics.GetBallScript().SetBallFloor(CalculateReboundFloor());
    }

    public void StartBlock()
    {
        physics.SetVelocity(CalculateShotBlockVelocity());
    }

    public void Update()
    {
        //Actually update the position
        Vector2 newVelocity = physics.GetVelocity() + Physics2D.gravity * Time.deltaTime;
        physics.SetVelocity(newVelocity);

        Vector2 movement = physics.GetVelocity() * Time.deltaTime;
        physics.Move(movement);

        //If player back on ground, bounce or stop
        if (physics.GetBallTransform().position.y < physics.GetBallScript().GetFloor().y) {
            //Set the ball to the floor and x velocity to 0 so it bounces straight up
            physics.SetPosition(new Vector2(physics.GetBallTransform().position.x, physics.GetBallScript().GetFloor().y));

            newVelocity = physics.GetVelocity();
            newVelocity.x = 0;
            physics.SetVelocity(newVelocity);

            if (Mathf.Abs(physics.GetVelocity().y) < minimumBounceVelocity) {
                physics.GetBallScript().SetBallState(BallScript.BallState.ON_GROUND);
            }
            else {
                newVelocity = physics.GetVelocity() * -1 * ballBounceFactor;
                physics.SetVelocity(newVelocity);
            }
        }

        CheckIfRebounded();
    }

    private void CheckIfRebounded()
    {
        RaycastHit2D[] collisions = new RaycastHit2D[16];

        int count = physics.GetBallCollider().Cast(Vector2.zero, rbndContactFilter, collisions, 0);

        //For every player the ball collided with
        for (int i = 0; i < count; i++) {

            PlayerScript testPlayer = collisions[i].transform.GetComponent<PlayerScript>();

            //If the player is in an acceptable spot to rebound, rebound
            if (testPlayer.IsInReboundRange(physics.GetBallScript().GetFloor().y)) {
                physics.GetBallScript().GrabBall(testPlayer);
                return;
            }
        }
    }

    //Calculate the velocity of the ball bouncing off the goal
    private Vector2 CalculateReboundVelocity()
    {
        //Bounce in the opposite direction of the goal
        Vector2 bounceDirection = physics.GetTargetGoal().isRightGoal ? new Vector2(-1, 1) : new Vector2(1, 1);

        //Bounce porportional to the shooting speed dampened
        return (bounceDirection * Mathf.Abs(physics.GetShootingSpeed()) * .5f);
    }

    //Calculate the position the ball should land on a missed shot
    private Vector2 CalculateReboundFloor()
    {
        Vector2 underBasket = physics.GetTargetGoal().underBasket.position;
        Vector2 shootingPlayerPos = physics.GetBallScript().GetCurrentPlayer().GetFrontPoint().position;
        float reboundFloorY;

        //If player is below the basket
        if (shootingPlayerPos.y < underBasket.y) {
            //Pick a random value between the player and the basket
            reboundFloorY = Random.Range(shootingPlayerPos.y, underBasket.y);
        }
        //If player is above the basket
        else {
            //Pick a random value between the basket and the player
            reboundFloorY = Random.Range(underBasket.y, shootingPlayerPos.y);
        }

        return new Vector2(0, reboundFloorY);
    }

    private Vector2 CalculateShotBlockVelocity()
    {
        //Generate a new velocity within given ranges
        Vector2 blockedVelocity = new Vector2(Random.Range(minBlockVelocityX, maxBlockVelocityX), blockVelocityY);

        //Flip the velocity if necessary
        if (physics.GetTargetGoal().isRightGoal) blockedVelocity.x *= -1;

        return blockedVelocity;
    }
}
