using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffenseTargetPoints : MonoBehaviour
{
    public TargetScript[] TargetList {get; private set;}

    public TargetScript TopCorner;
    public TargetScript TopWing;
    public TargetScript Key;
    public TargetScript BotWing;
    public TargetScript BotCorner;

    private void Awake()
    {
        TargetList = new TargetScript[] {
            TopCorner, TopWing, Key, BotWing, BotCorner
        };
    }
}
