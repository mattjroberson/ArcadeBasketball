using UnityEngine;

public abstract class BallPhysicsType
{
    protected BallPhysicsAttributesSO fields;
    protected BallScript ball;
    protected Vector2 velocity;
    protected Vector2 Position { get { return transform.position; } set { transform.position = value; } }

    private readonly Transform transform;

    public BallPhysicsType(BallScript ball)
    {
        this.ball = ball;
        transform = ball.transform;
        fields = ball.Fields;
    }

    public abstract void Update();

    protected void Move(Vector3 movement) { transform.position += movement; }

}
