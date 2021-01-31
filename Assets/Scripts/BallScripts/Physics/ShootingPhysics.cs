using UnityEngine;
using Extensions;
public class ShootingPhysics : BallPhysicsType
{    
    public GoalScript TargetGoal { get; set; }
    private Vector2 targetPos;

    public float ShootingSpeed { get; set; }
    private float[] trajectory;

    private PlayerScript shooter;
    private bool madeShot;

    private bool shotRightOfGoal;
    private float shotDistFromCenter;

    public ShootingPhysics(BallScript ball) : base(ball)
    {
        madeShot = false;
    }

    public void StartShot(PlayerScript shooter, GoalScript targetGoal, bool madeShot)
    {
        TargetGoal = targetGoal;
        targetPos = targetGoal.basketCenter.position;

        this.shooter = shooter;
        this.madeShot = madeShot;

        shotRightOfGoal = shooter.States.FloorPosition.x > targetPos.x;
        shotDistFromCenter = Position.y - targetPos.y;

        //Calculate the physics of the shot
        trajectory = TrajectoryScript.CalculateTrajectory(Position, targetPos,
            fields.ShotAngle, fields.ShotAngleXPercent);

        ShootingSpeed = CalculateShootingSpeed();
    }

    public void CheckForBlock(PlayerScript shooter, PlayerScript defender)
    {
        if (IsInBlockRange(shooter, defender, fields)) ball.OnShotBlocked();
    }

    public override void Update()
    {
        //Basketball trajectory is a simple parabola y = ax^2+bx+c
        float newX = Position.x + (ShootingSpeed * Time.deltaTime);
        float newY = (trajectory[0] * newX * newX) + (trajectory[1] * newX) + trajectory[2];

        Position = new Vector2(newX, newY);

        if (IsShotFinished()) FinishShot();
    }

    private float CalculateShootingSpeed()
    {
        float shootingSpeed;
        float distFromGoal = Mathf.Abs(targetPos.x - Position.x);

        //Map the speed of the ball to the x distance the ball is away from the basket
        if (distFromGoal < fields.ShotHalfCourtPos) {
            //If player is within half court, map to just half court
            shootingSpeed = distFromGoal.Remap(0, fields.ShotHalfCourtPos, fields.MinShotSpeed, fields.MaxRegShotSpeed);
        }
        else {
            //If player is in the backcourt, map to backcourt
            shootingSpeed = distFromGoal.Remap(fields.ShotHalfCourtPos, 
                fields.ShotFarthestPos, fields.MaxRegShotSpeed, fields.MaxShotSpeed);
        }

        //Scale the speed of the ball based on y distance the ball is away from the basketball
        float shotYModifier = (shotDistFromCenter < 0) ? fields.ShotUpperSpeedMod : fields.ShotLowerSpeedMod;
        shootingSpeed -= Mathf.Abs(shotDistFromCenter) * shotYModifier;

        //Flip the direction of the speed if shooting behind the goal
        if (shotRightOfGoal == true) { shootingSpeed *= -1; }

        return shootingSpeed;
    }

    //Returns true if the defender is between the shooter & the basket and close to shooter
    private bool IsInBlockRange(PlayerScript shooter, PlayerScript defender, BallPhysicsAttributesSO fields)
    {
        Vector2 p1 = shooter.States.FloorPosition;
        Vector2 p2 = TargetGoal.baseOfGoal.position;
        Vector2 p3 = defender.States.FloorPosition;

        Vector2 a1 = p1 - p2;
        Vector2 a2 = p3 - p2;
        Vector3 a3 = p3 - p1;

        float angle = Vector2.Angle(a1, a2);

        //In path
        if (Mathf.Abs(angle) < fields.MaxBlockableAngle) {
            //Close to player, block
            if (a3.magnitude < fields.MaxBlockableDistance) return true;
        }

        return false;
    }

    private bool IsShotFinished()
    {
        //Get the distance to the goal
        Vector2 distanceToGoal = targetPos - Position;

        //If the shot started to the right, flip the math
        if (shotRightOfGoal == true) {
            distanceToGoal = new Vector2(distanceToGoal.x * -1, distanceToGoal.y);
        }

        //If the ball has reached  the goal
        return (distanceToGoal.x <= 0 && distanceToGoal.y >= 0);
    }

    private void FinishShot()
    {
        Position = targetPos;

        if (madeShot) ball.DropFromGoal(TargetGoal);
        else ball.BounceOffGoal(TargetGoal, shooter.States.FloorPosition, ShootingSpeed);

        ball.FinishShot();
    }
}

namespace Extensions
{
    public static class ExtensionMethods
    {
        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

    }
}
