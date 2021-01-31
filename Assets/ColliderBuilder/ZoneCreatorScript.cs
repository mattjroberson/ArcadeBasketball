using System;
using System.Collections.Generic;
using UnityEngine;

public class ZoneCreatorScript : MonoBehaviour
{
    public PointsScript pointsContainer;
    public string pointNamesString;

    private PolygonCollider2D polygonCollider;
    private string[] pointNameList;

    private List<Vector2> positions;
    private List<PointScript> points;

    private void Start()
    {
        if (pointsContainer == null) throw new Exception("Remember to Attach Point Container!");
        polygonCollider = GetComponent<PolygonCollider2D>();
        CalculateVertices();

        if (polygonCollider == null) {
            GenerateCollider();
        }
    }

    private void CalculateVertices()
    {
        pointNameList = pointNamesString.Split(',');
        positions = new List<Vector2>();
        points = new List<PointScript>();

        foreach (string pointName in pointNameList) {
            PointScript point = pointsContainer.points[int.Parse(pointName)];

            positions.Add(point.transform.position);
            points.Add(point);
        }
    }

    private void GenerateCollider()
    {
        if(polygonCollider == null) polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
        polygonCollider.pathCount = 1;

        polygonCollider.SetPath(0, positions.ToArray()); 
    }

    private void Update()
    {
        if (polygonCollider == null) return;

        for(int i = 0; i < points.Count; i++) {
            if((Vector2)points[i].transform.position != positions[i]) {
                positions[i] = points[i].transform.position;
                polygonCollider.SetPath(0, positions.ToArray());
                break;
            }
        }
    }
}
