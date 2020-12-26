using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPhysicsScript : MonoBehaviour
{
    private BallScript basketball;
    private GoalScript targetGoal;

    private CircleCollider2D ballCollider;
    private ContactFilter2D ballContactFilter;

    private PassingPhysics passingPhysics;
    private ShootingPhysics shootingPhysics;
    private LooseBallPhysics looseBallPhysics;

    private Vector2 velocity;
    private Vector2 target;

    public void Awake()
    {
        ballContactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("BallCollider")));
        ballContactFilter.useLayerMask = true;
        ballContactFilter.useTriggers = false;
    }

    public void Start()
    {
        basketball = GetComponentInParent<BallScript>();
        ballCollider = GetComponentInParent<CircleCollider2D>();

        passingPhysics = new PassingPhysics(this);
        shootingPhysics = new ShootingPhysics(this);
        looseBallPhysics = new LooseBallPhysics(this);
    }

    public void UpdatePhysics()
    {
        switch (BallScript.GetBallState()) {
            case BallScript.BallState.PASSING:
                passingPhysics.Update();
                break;
            case BallScript.BallState.SHOOTING:
                shootingPhysics.Update();
                break;
            case BallScript.BallState.LOOSE:
                looseBallPhysics.Update();
                break;
        }
    }

    //Return the time till the loose ball hits the floor
    public float CalculateTimeTillGround()
    {
        float init_velocity_y = velocity.y;
        float displacement_y = Mathf.Abs(basketball.GetFloor().y - transform.position.y);

        float discriminant = Mathf.Pow(init_velocity_y, 2) + (-4 * -4.9f * displacement_y);
        float numerator = -Mathf.Sqrt(discriminant) - init_velocity_y;
        return numerator / Physics2D.gravity.y;
    }

    //Return the x position of the ball a given time
    public float CalculateXPositionAtTime(float time)
    {
        return transform.position.x + (velocity.x * time);
    }

    public void SetPosition(Vector2 position) { transform.position = position; }

    public void SetLocalPosition(Vector2 localPosition) { transform.parent.localPosition = localPosition; }

    public void Move(Vector3 movement) { transform.parent.position += movement; }

    public Vector2 GetVelocity() { return velocity; }

    public void SetVelocity(Vector2 newVelocity) { velocity = newVelocity; }

    public Vector2 GetTarget() { return target; }

    public void SetTarget(Vector2 newTarget) { target = newTarget; }

    public CircleCollider2D GetBallCollider() { return ballCollider; }

    public Transform GetBallTransform() { return basketball.transform;  }

    public BallScript GetBallScript() { return basketball;  }

    public ContactFilter2D GetBallContactFilter() { return ballContactFilter; }

    public GoalScript GetTargetGoal() { return targetGoal; }

    //Sub physics extension methods
    public void StartBlock() { looseBallPhysics.StartBlock();  }

    public void StartRebound() { looseBallPhysics.StartRebound();  }

    public void StartPass(PlayerScript target) { passingPhysics.StartPass(target); }

    public float GetShootingSpeed() { return shootingPhysics.GetShootingSpeed(); }

    public void StartShot(GoalScript targetGoal) {
        this.targetGoal = targetGoal;
        shootingPhysics.StartShot(targetGoal);
    }
}

public static class ExtensionMethods
{

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

}