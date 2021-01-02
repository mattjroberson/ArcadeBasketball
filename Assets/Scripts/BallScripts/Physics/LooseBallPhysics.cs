using UnityEngine;

public class LooseBallPhysics : BallPhysicsType
{
    private Vector2 ballFloor;

    public LooseBallPhysics(BallScript ball) : base(ball) {}

    public void BounceOffBlock(GoalScript targetGoal, Vector2 shooterFootPos)
    {
        float minXVel = fields.MinBlockVelocityX;
        float maxXVel = fields.MaxBlockVelocityX;

        Vector2 blockedVelocity = new Vector2(Random.Range(minXVel, maxXVel), fields.BlockVelocityY);

        //Flip the velocity if necessary
        if(targetGoal.isRightGoal) blockedVelocity.x *= -1;

        velocity = blockedVelocity;
        ballFloor = shooterFootPos;
    }

    public void BounceOffGoal(GoalScript targetGoal, Vector2 shooterPos, float shotSpeed)
    {
        bool isRight = targetGoal.isRightGoal;

        float underBasketY = targetGoal.underBasket.position.y;
        float shooterY = shooterPos.y;

        velocity = CalculateReboundVelocity(isRight, shotSpeed);
        ballFloor = CalculateReboundFloor(shooterY, underBasketY);
    }

    public void DropFromGoal(GoalScript targetGoal)
    {
        velocity = Vector2.zero;
        ballFloor = targetGoal.underBasket.position;
    }

    public void CheckForRebound(PlayerScript player)
    {
        if (IsInReboundRange(player.FrontPoint)) ball.OnBallPickup(player);
    }

    public void DrawShadow(GameObject shadowPrefab, float ballSize)
    {
        float timeTillGround = CalculateTimeTillGround();

        Vector2 shadowPos = new Vector2();
        shadowPos.x = CalculateXPositionAtTime(timeTillGround);
        shadowPos.y = ballFloor.y - (ballSize / 2);

        GameObject shadow = Object.Instantiate(shadowPrefab, shadowPos, Quaternion.identity);
        shadow.GetComponent<BallShadowScript>().Focus(timeTillGround);
    }

    public override void Update()
    {
        Vector2 newVelocity = velocity + Physics2D.gravity * Time.deltaTime;
        velocity = newVelocity;

        Vector2 movement = velocity * Time.deltaTime;
        Move(movement);

        if (Position.y < ballFloor.y) OnBallHitGround();
    }

    private void OnBallHitGround()
    {        
        //Set the ball to the floor and x velocity to 0 so it bounces straight up
        Position = new Vector2(Position.x, ballFloor.y);

        Vector2 newVelocity = velocity;
        newVelocity.x = 0;
        velocity = newVelocity;

        if (Mathf.Abs(velocity.y) > fields.MinBounceVelocity) BounceOffFloor();
        else ball.CompleteBouncing();
    }

    private void BounceOffFloor()
    {
        velocity = velocity * -1 * fields.BounceFactor;
    }
    
    private bool IsInReboundRange(FrontPointScript frontPoint)
    {
        float playerY = frontPoint.FloorPosition.y;

        bool belowTopMargin = playerY > ballFloor.y - fields.ReboundFloorMargin;
        bool aboveBotMargin = playerY < ballFloor.y + fields.ReboundFloorMargin;

        return (belowTopMargin && aboveBotMargin);
    }

    //Calculate the velocity of the ball bouncing off the goal
    private Vector2 CalculateReboundVelocity(bool isRightGoal, float shootingSpeed)
    {
        //Bounce in the opposite direction of the goal
        Vector2 bounceDirection = isRightGoal ? new Vector2(-1, 1) : new Vector2(1, 1);

        //Bounce porportional to the shooting speed dampened
        return (bounceDirection * Mathf.Abs(shootingSpeed) * fields.ShotSpeedDamper);
    }

    //Calculate the position the ball should land on a missed shot
    private Vector2 CalculateReboundFloor(float shootingPlayerY, float underBasketY)
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

    private float CalculateTimeTillGround()
    { 
        //time = [-sqrt(vel^2 + (2 * -gravity * dist)) - vel] / gravity
        float displacementY = Mathf.Abs(ballFloor.y - Position.y);
        float initVelocityY = velocity.y;
        
        float discriminant = Mathf.Pow(initVelocityY, 2) + (2 * -Physics2D.gravity.y * displacementY);
        float numerator = -Mathf.Sqrt(discriminant) - initVelocityY;
        return numerator / Physics2D.gravity.y;
    }

    private float CalculateXPositionAtTime(float time)
    {
        return Position.x + (velocity.x * time);
    }
}
