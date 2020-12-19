using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamScript : MonoBehaviour
{
    public enum CourtSide { LEFT, RIGHT };
    public CourtSide currentSide;

    private SideManager sideManager;

    void Awake()
    {
        string side = currentSide == CourtSide.LEFT ? "LeftSide" : "RightSide";
        sideManager = GameObject.Find(side).GetComponent<SideManager>();
    }

    public SideManager getCurrentSide() { return sideManager; }
}
