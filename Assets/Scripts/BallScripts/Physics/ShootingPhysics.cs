using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingPhysics
{
    private BallPhysicsScript physics;
    private GoalScript targetGoal;

    private float[] trajectory;
    private float shootingSpeed;

    public float maxRegShotSpeed;
    public float maxShotSpeed;
    public float minShotSpeed;
    public float shotAngle;
    public float shotAngleXPercent;
    public float shotLowerYModifier;
    public float shotUpperYModifier;
    public float shotHalfCourtPos;
    public float shotFarthestPos;

    private float maxBlockableAngle;
    private float maxBlockableDistance;

    private float shotDistFromCenter;
    private bool shotRightOfGoal;

    public ShootingPhysics(BallPhysicsScript physics)
    {
        this.physics = physics;

        maxRegShotSpeed = 4f;
        maxShotSpeed = 5f;
        minShotSpeed = 2.25f;

        //Ball should approach 45 degrees 90% of the way to the goal
        shotAngle = Mathf.PI / 4;
        shotAngleXPercent = .9f;

        shotHalfCourtPos = 5.5f;
        shotFarthestPos = 13f;
        shotLowerYModifier = .4f;
        shotUpperYModifier = .6f;

        //Blocking Parameters
        maxBlockableAngle = 10f;
        maxBlockableDistance = 1f;
    }

    public void Update()
    {
        //Basketball trajectory is a simple parabola y = ax^2+bx+c
        float newX = physics.GetBallTransform().position.x + (shootingSpeed * Time.deltaTime);
        float newY = (trajectory[0] * newX * newX) + (trajectory[1] * newX) + trajectory[2];

        physics.SetPosition(new Vector2(newX, newY));

        CheckIfShotFinished();
        CheckIfShotBlocked();
    }

    public void StartShot(GoalScript targetGoal)
    {
        this.targetGoal = targetGoal;

        //Set the target position
        physics.SetTarget(targetGoal.basketCenter.position);

        //Calculate the physics of the shot
        trajectory = TrajectoryScript.CalculateTrajectory(physics.GetBallTransform().position, physics.GetTarget(), shotAngle, shotAngleXPercent);

        CalculateShootingSpeed();
    }

    private void CalculateShootingSpeed()
    {
        float distFromGoal = Mathf.Abs(physics.GetTarget().x - physics.GetBallTransform().position.x);

        //Map the speed of the ball to the x distance the ball is away from the basket
        if (distFromGoal < shotHalfCourtPos) {
            //If player is within half court, map to just half court
            shootingSpeed = distFromGoal.Remap(0, shotHalfCourtPos, minShotSpeed, maxRegShotSpeed);
        }
        else {
            //If player is in the backcourt, map to backcourt
            shootingSpeed = distFromGoal.Remap(shotHalfCourtPos, shotFarthestPos, maxRegShotSpeed, maxShotSpeed);
        }

        //Scale the speed of the ball based on y distance the ball is away from the basketball
        float shotYModifier = (shotDistFromCenter < 0) ? shotUpperYModifier : shotLowerYModifier;
        shootingSpeed -= Mathf.Abs(shotDistFromCenter) * shotYModifier;

        //Flip the direction of the speed if shooting behind the goal
        if (shotRightOfGoal == true) { shootingSpeed *= -1; }
    }

    private void CheckIfShotBlocked()
    {
        RaycastHit2D[] collisions = new RaycastHit2D[16];

        int count = physics.GetBallCollider().Cast(Vector2.zero, physics.GetBallContactFilter(), collisions, 0);

        for (int i = 0; i < count; i++) {
            //Get a reference to the player potentially blocking
            PlayerScript defender = collisions[i].transform.GetComponent<PlayerScript>();

            //Don't count the shooter
            if (defender == physics.GetBallScript().GetCurrentPlayer()) continue;

            //If defender is in a correct position, block it
            if (IsDefenderInShotPath(defender) == true) {
                physics.GetBallScript().BlockShot();
            }
        }
    }

    //Returns true if the defender is between the shooter & the basket and close to shooter
    private bool IsDefenderInShotPath(PlayerScript defender)
    {
        Vector2 p1 = physics.GetBallScript().GetCurrentPlayer().GetFrontPoint().position;
        Vector2 p2 = targetGoal.baseOfGoal.position;
        Vector2 p3 = defender.GetFrontPoint().position;

        Vector2 a1 = p1 - p2;
        Vector2 a2 = p3 - p2;
        Vector3 a3 = p3 - p1;

        float angle = Vector2.Angle(a1, a2);

        //In path
        if (Mathf.Abs(angle) < maxBlockableAngle) {
            //Close to player, block
            if (a3.magnitude < maxBlockableDistance) {
                return true;
            }
        }

        return false;
    }

    private void CheckIfShotFinished()
    {
        //Get the distance to the goal
        Vector2 distanceToGoal = physics.GetTarget() - (Vector2)physics.GetBallTransform().position;

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
        //If made, ball just drops beneath the basket
        if (physics.GetBallScript().GetMadeShot() == true) {
            physics.SetVelocity(Vector2.zero);
            physics.GetBallScript().SetBallFloor(targetGoal.underBasket.position);
        }
        //If missed, ball should bounce off goal
        else {
            physics.StartRebound();
        }

        physics.SetPosition(physics.GetTarget());
        physics.GetBallScript().HandleShotFinished();
    }

    public float GetShootingSpeed() { return shootingSpeed; }
}
