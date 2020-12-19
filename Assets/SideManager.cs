using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideManager : MonoBehaviour
{
    private GoalScript goalScript;
    private Transform shotZones;

    // Start is called before the first frame update
    void Awake()
    {
        goalScript = GetComponentInChildren<GoalScript>();
        shotZones = transform.Find("ShotZones");
    }

    public GoalScript getGoalScript() { return goalScript; }

    public Transform getShotZones() { return shotZones; }

}
