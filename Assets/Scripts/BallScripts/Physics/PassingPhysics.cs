using UnityEngine;

public class PassingPhysics
{
    private BallPhysicsScript physics;

    public PassingPhysics(BallPhysicsScript physics)
    {
        this.physics = physics;
    }

    public void Update()
    {
        physics.Move(physics.Velocity * Time.deltaTime);
    }

    public void StartPass(PlayerScript targetPlayer)
    {
        Vector2 target = targetPlayer.Hands.position;

        Vector2 newVelocity = CalculateVelocity(target);
        physics.Velocity = newVelocity;
    }

    private Vector2 CalculateVelocity(Vector2 target)
    {
        Vector2 straight_velocity = (target - (Vector2)physics.transform.position);
        straight_velocity /= straight_velocity.magnitude;

        return straight_velocity * physics.Fields.PassingSpeed;
    }
}
