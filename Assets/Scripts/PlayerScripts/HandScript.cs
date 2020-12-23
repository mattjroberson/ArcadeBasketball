using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandScript : MonoBehaviour
{

    private BoxCollider2D handsCollider;

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

    private void SetHandsUp() { transform.localPosition = handsUp; }

    private void SetHandsDown() { transform.localPosition = handsDown; }

    private void SetHandsDunking() { transform.localPosition = handsDunking;}
}
