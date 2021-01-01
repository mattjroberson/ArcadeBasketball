using UnityEngine;

public class LooseBallPhysics 
{
    private BallPhysicsScript physics;
    private BallScript ball;

    public LooseBallPhysics(BallPhysicsScript physics, BallScript ball)
    {
        this.physics = physics;
        this.ball = ball;
    }

    public void BounceOffBlock()
    {
        //Generate a new velocity within given ranges
        Vector2 blockedVelocity = new Vector2(Random.Range(physics.Fields.MinBlockVelocityX,
            physics.Fields.MaxBlockVelocityX), physics.Fields.BlockVelocityY);

        //Flip the velocity if necessary
        if(physics.Shooting.TargetGoal.isRightGoal) blockedVelocity.x *= -1;

        physics.Velocity = blockedVelocity;
        ball.BallFloor = ball.CurrentHandler.FrontPoint.FloorPosition;
    }

    public void BounceOffGoal()
    {
        bool isRight = physics.Shooting.TargetGoal.isRightGoal;
        float shotSpeed = physics.Shooting.ShootingSpeed;

        float underBasketY = physics.Shooting.TargetGoal.underBasket.position.y;
        float shooterY = ball.CurrentHandler.FrontPoint.FloorPosition.y;

        physics.Velocity = CalculateReboundVelocity(isRight, shotSpeed);
        ball.BallFloor = CalculateReboundFloor(shooterY, underBasketY);
    }

    public void DropFromGoal(Vector2 ballFloor)
    {
        physics.Velocity = Vector2.zero;
        ball.BallFloor = ballFloor;
    }

    public void Update()
    {
        Vector2 newVelocity = physics.Velocity + Physics2D.gravity * Time.deltaTime;
        physics.Velocity = newVelocity;

        Vector2 movement = physics.Velocity * Time.deltaTime;
        physics.Move(movement);

        //If player back on ground, bounce or stop
        if (physics.transform.position.y <  ball.BallFloor.y) {
            //Set the ball to the floor and x velocity to 0 so it bounces straight up
            physics.SetPosition(new Vector2(physics.transform.position.x, ball.BallFloor.y));

            newVelocity = physics.Velocity;
            newVelocity.x = 0;
            physics.Velocity = newVelocity;

            if (Mathf.Abs(physics.Velocity.y) < physics.Fields.MinBounceVelocity) {
                ball.State = BallScript.BallState.ON_GROUND;
            }
            else {
                newVelocity = physics.Velocity * -1 * physics.Fields.BounceFactor;
                physics.Velocity = newVelocity;
            }
        }
    }
    public static bool IsInReboundRange(FrontPointScript frontPoint, Vector2 ballFloor)
    {
        float playerY = frontPoint.FloorPosition.y;
        float rbndFloorMargin = .4f;

        float rbndFloorY = ballFloor.y;

        //If player is above floor - margin and above floor + margin return true
        if (playerY > rbndFloorY - rbndFloorMargin && playerY < rbndFloorY + rbndFloorMargin)
        {
            return true;
        }
        else return false;
    }

    //Calculate the velocity of the ball bouncing off the goal
    public static Vector2 CalculateReboundVelocity(bool isRightGoal, float shootingSpeed)
    {
        //Bounce in the opposite direction of the goal
        Vector2 bounceDirection = isRightGoal ? new Vector2(-1, 1) : new Vector2(1, 1);

        //Bounce porportional to the shooting speed dampened
        return (bounceDirection * Mathf.Abs(shootingSpeed) * .5f);
    }

    //Calculate the position the ball should land on a missed shot
    public static Vector2 CalculateReboundFloor(float shootingPlayerY, float underBasketY)
    {
        float reboundFloorY;

        //If player is below the basket
        if (shootingPlayerY < underBasketY) {
            //Pick a random value between the player and the basket
            reboundFloorY = Random.Range(shootingPlayerY, underBasketY);
        }
        //If player is above the basket
        else {
            //Pick a random value between the basket and the player
            reboundFloorY = Random.Range(underBasketY, shootingPlayerY);
        }

        return new Vector2(0, reboundFloorY);
    }

    //TODO Fix Hardcoded values
    public static float CalculateTimeTillGround(float initVelocityY, Vector2 ballFloor, Vector2 position)
    {
        float displacementY = Mathf.Abs(ballFloor.y - position.y);

        float discriminant = Mathf.Pow(initVelocityY, 2) + (-4 * -4.9f * displacementY);
        float numerator = -Mathf.Sqrt(discriminant) - initVelocityY;
        return numerator / Physics2D.gravity.y;
    }

}
