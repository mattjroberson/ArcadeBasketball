﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShotMeter : MonoBehaviour
{
    private enum BarState { FILLING, DROPPING, PAUSED }
    private BarState barState;

    private ActionsScript actions;
    private AttributeScript attributes;
    private Slider meter;

    private Quaternion lockedRotation;

    private float fillSpeed;
    private float value;

    public void Start()
    {
        actions = transform.parent.parent.GetComponentInParent<ActionsScript>();
        attributes = transform.parent.parent.GetComponentInParent<AttributeScript>();

        meter = transform.GetComponent<Slider>();
        barState = BarState.PAUSED;
        value = meter.minValue;

        fillSpeed = (meter.maxValue - meter.minValue) / attributes.GetShotMeterSpeed() * 2;

        actions.events.onShootBegin += ShootBeginEvent;
        actions.events.onShootEnd += ShootEndEvent;
    }

    private void ShootBeginEvent()
    {
        barState = BarState.FILLING;
        value = meter.minValue;
    }

    private void ShootEndEvent()
    {
        barState = BarState.PAUSED;
        value = meter.minValue;
    }

    public void FixedUpdate()
    {
        switch (barState) {
            case BarState.DROPPING:
                value -= Time.deltaTime * fillSpeed;

                value = Mathf.Clamp(value, meter.minValue, meter.maxValue);
                meter.value = value;

                CheckEmpty();
                break;
            case BarState.FILLING:
                value += Time.deltaTime * fillSpeed;

                value = Mathf.Clamp(value, meter.minValue, meter.maxValue);
                meter.value = value;

                CheckFull();
                break;
        }
    }

    public float GetValue() { return value; }

    private void CheckFull()
    {
        if (value == meter.maxValue) {
            barState = BarState.DROPPING;
        }
    }

    private void CheckEmpty()
    {
        if (value == meter.minValue) {
            actions.events.WalkViolation();
        }
    }
}
