using UnityEngine;

public class PassingPhysics : BallPhysicsType
{
    public PassingPhysics(BallScript ball) : base(ball) { }

    public override void Update()
    {
        Move(velocity * Time.deltaTime);
    }

    public void StartPass(PlayerScript targetPlayer)
    {
        Vector2 target = targetPlayer.Hands.position;

        Vector2 newVelocity = CalculateVelocity(target);
        velocity = newVelocity;
    }

    private Vector2 CalculateVelocity(Vector2 target)
    {
        Vector2 straight_velocity = (target - Position);
        straight_velocity /= straight_velocity.magnitude;

        return straight_velocity * fields.PassingSpeed;
    }
}
