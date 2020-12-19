using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassingPhysics
{
    private BallPhysicsScript physics;

    private PlayerScript targetPlayer;
    private PlayerScript teammatePlayer;

    private float passingSpeed;

    public PassingPhysics(BallPhysicsScript physics)
    {
        this.physics = physics;
        passingSpeed = 4f;
    }

    public void Update()
    {
        float distance = physics.GetVelocity().magnitude;

        float distance_from_target = (physics.GetTarget() - (Vector2)physics.GetBallTransform().position).magnitude;
        float distance_to_travel = (physics.GetVelocity() * Time.deltaTime).magnitude;

        //If ball is going to reach the target next frame
        if (distance_from_target < distance_to_travel) {
            physics.SetVelocity(Vector2.zero);
            physics.SetPosition(physics.GetTarget());
            physics.GetBallScript().RecievePass(targetPlayer);
            return;
        }

        CheckIfStolen();

        physics.Move(physics.GetVelocity() * Time.deltaTime);
    }

    public void StartPass(PlayerScript targetPlayer)
    {
        this.targetPlayer = targetPlayer;
        teammatePlayer = targetPlayer.GetTeammate();

        physics.SetTarget(targetPlayer.GetHands().position);

        Vector2 newVelocity = CalculateVelocity(physics.GetTarget());
        physics.SetVelocity(newVelocity);
    }

    private Vector2 CalculateVelocity(Vector2 target)
    {
        Vector2 straight_velocity = (target - (Vector2)physics.GetBallTransform().position);
        straight_velocity /= straight_velocity.magnitude;

        return straight_velocity * passingSpeed;
    }

    private void CheckIfStolen()
    {
        RaycastHit2D[] collisions = new RaycastHit2D[16];

        int count = physics.GetBallCollider().Cast(Vector2.zero, physics.GetBallContactFilter(), collisions, 0);

        if (count > 0) {
            //Ignore if its the same player or the player passing to
            if (collisions[0].transform == targetPlayer.transform || collisions[0].transform == teammatePlayer.transform) return;

            PlayerScript defender = collisions[0].transform.GetComponent<PlayerScript>();

            physics.SetPosition(defender.GetHands().position);
            physics.GetBallScript().StealPass(defender, targetPlayer);
        }
    }
}
