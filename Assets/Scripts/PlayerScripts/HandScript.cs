using UnityEngine;

public class HandScript : MonoBehaviour
{
    private Vector2 handsUp;
    private Vector2 handsDown;
    private Vector2 handsDunking;

    private ActionsScript actions;

    void Start()
    {
        handsDown = transform.localPosition;
        handsUp = new Vector2(-.01f, 1.13f);
        handsDunking = new Vector2(.5f, 1f);

        actions = GetComponentInParent<ActionsScript>();
        actions.events.onJumpBegin += SetHandsUp;
        actions.events.onJumpEnd += SetHandsDown;
        actions.events.onDunkBegin += SetHandsDunking;
        actions.events.onDunkEnd += SetHandsDown;
    }

    private void SetHandsUp(float scalar) { transform.localPosition = handsUp; }

    private void SetHandsDown() { transform.localPosition = handsDown; }

    private void SetHandsDunking() { transform.localPosition = handsDunking;}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        actions.HandTouchBall();
    }
}
