using UnityEngine;

public class ShootingPhysics
{
    private BallPhysicsScript physics;
    private BallScript ball;
    
    public GoalScript TargetGoal { get; set; }
    private Vector2 targetPos;

    private float[] trajectory;
    
    public float ShootingSpeed { get; set; }

    private bool shotRightOfGoal;
    private float shotDistFromCenter;

    private bool madeShot;

    public ShootingPhysics(BallPhysicsScript physics, BallScript ball)
    {
        this.physics = physics;
        this.ball = ball;

        madeShot = false;
    }

    public void Update()
    {
        //Basketball trajectory is a simple parabola y = ax^2+bx+c
        float newX = physics.transform.position.x + (ShootingSpeed * Time.deltaTime);
        float newY = (trajectory[0] * newX * newX) + (trajectory[1] * newX) + trajectory[2];

        physics.SetPosition(new Vector2(newX, newY));

        CheckIfShotFinished();
    }

    public void StartShot(GoalScript targetGoal, bool madeShot)
    {
        TargetGoal = targetGoal;
        targetPos = targetGoal.basketCenter.position;

        this.madeShot = madeShot;

        shotRightOfGoal = ball.CurrentHandler.FrontPoint.FloorPosition.x > targetPos.x;
        shotDistFromCenter = physics.transform.position.y - targetPos.y;

        //Calculate the physics of the shot
        trajectory = TrajectoryScript.CalculateTrajectory(physics.transform.position, targetPos, 
            physics.Fields.ShotAngle, physics.Fields.ShotAngleXPercent);

        CalculateShootingSpeed();
    }

    private void CalculateShootingSpeed()
    {
        float distFromGoal = Mathf.Abs(targetPos.x - physics.transform.position.x);

        //Map the speed of the ball to the x distance the ball is away from the basket
        if (distFromGoal < physics.Fields.ShotHalfCourtPos) {
            //If player is within half court, map to just half court
            ShootingSpeed = distFromGoal.Remap(0, physics.Fields.ShotHalfCourtPos, physics.Fields.MinShotSpeed, physics.Fields.MaxRegShotSpeed);
        }
        else {
            //If player is in the backcourt, map to backcourt
            ShootingSpeed = distFromGoal.Remap(physics.Fields.ShotHalfCourtPos, 
                physics.Fields.ShotFarthestPos, physics.Fields.MaxRegShotSpeed, physics.Fields.MaxShotSpeed);
        }

        //Scale the speed of the ball based on y distance the ball is away from the basketball
        float shotYModifier = (shotDistFromCenter < 0) ? physics.Fields.ShotUpperSpeedMod : physics.Fields.ShotLowerSpeedMod;
        ShootingSpeed -= Mathf.Abs(shotDistFromCenter) * shotYModifier;

        //Flip the direction of the speed if shooting behind the goal
        if (shotRightOfGoal == true) { ShootingSpeed *= -1; }
    }

    //Returns true if the defender is between the shooter & the basket and close to shooter
    public static bool IsInBlockRange(PlayerScript shooter, PlayerScript defender, GoalScript goal, BallPhysicsAttributesSO fields)
    {
        Vector2 p1 = shooter.FrontPoint.FloorPosition;
        Vector2 p2 = goal.baseOfGoal.position;
        Vector2 p3 = defender.FrontPoint.FloorPosition;

        Vector2 a1 = p1 - p2;
        Vector2 a2 = p3 - p2;
        Vector3 a3 = p3 - p1;

        float angle = Vector2.Angle(a1, a2);

        //In path
        if (Mathf.Abs(angle) < fields.MaxBlockableAngle) {
            //Close to player, block
            if (a3.magnitude < fields.MaxBlockableDistance) {
                return true;
            }
        }

        return false;
    }

    private void CheckIfShotFinished()
    {
        //Get the distance to the goal
        Vector2 distanceToGoal = targetPos - (Vector2)physics.transform.position;

        //If the shot started to the right, flip the math
        if (shotRightOfGoal == true) {
            distanceToGoal = new Vector2(distanceToGoal.x * -1, distanceToGoal.y);
        }

        //If the ball has reached  the goal
        if (distanceToGoal.x <= 0 && distanceToGoal.y >= 0) {
            FinishShot();
        }
    }

    private void FinishShot()
    {
        physics.SetPosition(targetPos);

        if (madeShot) {
            physics.LooseBall.DropFromGoal(TargetGoal.underBasket.position);
        }
        else {
            physics.LooseBall.BounceOffGoal();
        }

        ball.FinishShot();
    }
}
