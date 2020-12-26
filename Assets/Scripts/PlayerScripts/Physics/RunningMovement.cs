using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningMovement : MovementType
{
    private Vector2 targetVelocity;

    private BoxCollider2D footCollider;

    private float minimumMoveDistance;
    private float minimumFlipDistance;
    private float ySpeedModifier;
    private float xSlopeVelocity;
    private float xSlopeModifier;

    private float feetHitMargin;

    public RunningMovement(PhysicsScript physics, PlayerScript player, ActionsScript actions) : base(physics, player, actions){
        targetVelocity = Vector2.zero;

        footCollider = physics.transform.Find("ColliderContainer").Find("FootCollider").GetComponent<BoxCollider2D>();

        minimumMoveDistance = 0.001f;
        minimumFlipDistance = 0.02f;
        ySpeedModifier = 1.5f;
        xSlopeModifier = 1.5f;

        feetHitMargin = 0.01f;
    }

    public override void Update()
    {
        targetVelocity = ComputeTargetVelocity();

        currentVelocity.x = (targetVelocity.x == 0) ? xSlopeVelocity : targetVelocity.x;
        currentVelocity.y = targetVelocity.y / ySpeedModifier;

        Vector2 delta_position = currentVelocity * Time.deltaTime;

        Vector2 move = Vector2.right * delta_position.x;
        ApplyRunningMovement(move, false);

        move = Vector2.up * delta_position.y;
        ApplyRunningMovement(move, true);
    }

    //Get the velocity as the players direction scaled by its speed
    private Vector2 ComputeTargetVelocity()
    {
        return player.GetCurrentMoveDirection() * player.GetCurrentSpeed();
    }

    //TODO Major refactoring needed with this monster
    //Actually apply the movement to the player, to each axis individually
    private void ApplyRunningMovement(Vector2 newMove, bool isYMovement)
    {
        Vector2 direction = newMove.normalized;
        float distance = newMove.magnitude;

        RaycastHit2D[] collisionArray = new RaycastHit2D[16];

        //If player moved enough to register
        if (distance > minimumMoveDistance)
        {
            //player.AddDistanceMoved(distance);

            //Get a count of how many things the player will collide with in this movement
            int collisionCount = footCollider.Cast(direction, groundContactFilter, collisionArray, distance + feetHitMargin);

            for (int i = 0; i < collisionCount; i++)
            {
                Vector2 currentNormal = collisionArray[i].normal;

                //Allows sliding along slopes
                if (isYMovement && currentNormal.y != 1 && currentNormal.y != -1)
                {
                    float yNormal = currentNormal.y;

                    //If player is on the left side, flip the y normal
                    if (physics.transform.position.x < 0)
                    {
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
            if (collisionCount == 0)
            {
                xSlopeVelocity = 0;
            }

            Vector2 adjustedMovement = newMove.normalized * distance;

            //Update the direction the player is facing. If movement is less than minimum, pass 0
            float xMovement = (Mathf.Abs(adjustedMovement.x) > minimumFlipDistance) ? adjustedMovement.x : 0;
            HandleOrientation(xMovement);

            //Update the position of the player
            physics.transform.position = physics.transform.position + (Vector3)adjustedMovement;

            //Check if the player ran over a ball on the court
            if (IsTouchingLooseBall(adjustedMovement))
            {
                actions.PickupLooseBall(physics.transform.GetComponent<PlayerScript>());
            }
        }
    }

    //TODO Refactor with a trigger on a ball script, and a game event 
    private bool IsTouchingLooseBall(Vector2 move)
    {
        if (BallScript.GetBallState() == BallScript.BallState.ON_GROUND)
        {

            RaycastHit2D[] collisionArray = new RaycastHit2D[16];

            int count = footCollider.Cast(move, ballContactFilter, collisionArray, move.magnitude);

            if (count > 0) return true;
        }

        return false;
    }
}
