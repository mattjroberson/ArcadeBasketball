using System;
using UnityEngine;

public class BallSortableSprite : MonoBehaviour, ISortableSprite
{
    private BallScript ball;

    private SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer => spriteRenderer;

    public float SortPosition => CurrentPosition();
    
    private Func<float> CurrentPosition;

    private float passFloorDisplacement;
    private float looseBallFloor;

    void Start()
    {
        ball = GetComponent<BallScript>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        CurrentPosition = OnTop;

        GameEvents.Instance.onPassSent += PassSentEvent;
        GameEvents.Instance.onPossessionChange += PossessionChangedEvent;
        GameEvents.Instance.onBallLoose += BallLooseEvent;
        BallEvents.Instance.onBallShot += ShotReleasedEvent;
    }

    private void PassSentEvent()
    {
        SpriteRenderer.enabled = true;
        passFloorDisplacement = transform.position.y -  ball.GetBallHandler().States.FloorPosition.y;
        CurrentPosition = Passing;
    }
    private void BallLooseEvent(Vector2 landingPos)
    {
        SpriteRenderer.enabled = true;
        looseBallFloor = landingPos.y;
        CurrentPosition = LooseBall;
    }
    private void ShotReleasedEvent(GoalScript goal, bool madeShot)
    {
        SpriteRenderer.enabled = true;
        CurrentPosition = OnTop;
    }
    private void PossessionChangedEvent(PlayerScript player)
    {
        SpriteRenderer.enabled = false;
    }

    private float OnTop() { return -1; }

    private float Passing() { return transform.position.y - passFloorDisplacement; }

    private float LooseBall() { return looseBallFloor; }
}

