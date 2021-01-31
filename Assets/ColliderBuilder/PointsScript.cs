using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsScript : MonoBehaviour
{
    public PointScript[] points;

    void Awake()
    {
        points = GetComponentsInChildren<PointScript>();
    }
}
