using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTransformScript : MonoBehaviour
{
    [SerializeField] private Vector2 worldPos;

    // Update is called once per frame
    void Update()
    {
        worldPos = transform.localPosition + transform.parent.position;
    }
}
