using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandScript : MonoBehaviour
{

    private BoxCollider2D handsCollider;

    private Vector2 handsUp;
    private Vector2 handsDown;
    private Vector2 handsDunking;

    void Start()
    {
        handsDown = transform.localPosition;
        handsUp = new Vector2(-.01f, 1.13f);
        handsDunking = new Vector2(.5f, 1f);
    }

    public void SetHandsUp() { transform.localPosition = handsUp; }

    public void SetHandsDown() { transform.localPosition = handsDown; }

    public void SetHandsDunking() { transform.localPosition = handsDunking;}
}
