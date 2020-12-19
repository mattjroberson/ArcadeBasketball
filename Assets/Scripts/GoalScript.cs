using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    public Transform basketCenter;
    public Transform underBasket;
    public Transform baseOfGoal;

    public bool isRightGoal;

    void Start()
    {
        basketCenter = transform.Find("basketCenter");
        underBasket = transform.Find("underBasket");
        baseOfGoal = transform.Find("baseOfGoal");
    }
}
