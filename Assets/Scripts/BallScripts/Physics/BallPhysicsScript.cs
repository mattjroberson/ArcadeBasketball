using UnityEngine;

public class BallPhysicsScript : MonoBehaviour
{
    [SerializeField] private BallPhysicsAttributesSO fields;
    public BallPhysicsAttributesSO Fields => fields;
    public PassingPhysics Passing { get; set; }
    public ShootingPhysics Shooting { get; set; }
    public LooseBallPhysics LooseBall { get; set; }

    public Vector2 Velocity { get; set; }

    private BallScript ball;

    public void Start()
    {
        ball = GetComponentInParent<BallScript>();

        Passing = new PassingPhysics(this);
        Shooting = new ShootingPhysics(this, ball);
        LooseBall = new LooseBallPhysics(this, ball);
    }

    public void UpdatePhysics()
    {
        switch (ball.State) {
            case BallScript.BallState.PASSING:
                Passing.Update();
                break;
            case BallScript.BallState.SHOOTING:
                Shooting.Update();
                break;
            case BallScript.BallState.LOOSE:
                LooseBall.Update();
                break;
        }
    }

    //Return the x position of the ball a given time
    public float CalculateXPositionAtTime(float time)
    {
        return transform.position.x + (Velocity.x * time);
    }

    public void SetPosition(Vector2 position) { transform.position = position; }

    public void Move(Vector3 movement) { transform.parent.position += movement; }
}

public static class ExtensionMethods
{

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

}