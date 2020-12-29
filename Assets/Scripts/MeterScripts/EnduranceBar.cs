using UnityEngine;
using UnityEngine.UI;

public class EnduranceBar : MonoBehaviour
{
    private enum BarState { FILLING, CONSUMING, STEADY, COOLDOWN }
    private BarState barState;

    private PlayerScript player;
    private ActionsScript actions;
    private Slider meter;

    private float coolDown;
    private float coolDownTime;
    private float fillSpeed;
    private float value;

    private Quaternion lockedRotation;

    void Awake()
    {
        lockedRotation = transform.rotation;

        coolDownTime = 1f;
        coolDown = 0f;
        fillSpeed = .5f;
    }

    void Start()
    {
        player = transform.parent.parent.GetComponentInParent<PlayerScript>();
        actions = transform.parent.parent.GetComponentInParent<ActionsScript>();
        meter = GetComponent<Slider>();

        value = player.Attributes.GetMaxEndurance();
        meter.maxValue = value;

        actions.events.onSprintBegin += SprintBeginEvent;
        actions.events.onSprintEnd += SprintEndEvent;

        actions.events.onShootBegin += ShootBeginEvent;
        actions.events.onShootEnd += ShootEndEvent;
    }

    void FixedUpdate()
    {
        switch (barState) {
            case BarState.CONSUMING:
                value -= Time.deltaTime;

                value = Mathf.Clamp(value, 0, meter.maxValue);
                meter.value = value;

                CheckEmpty();
                break;
            case BarState.FILLING:
                value += Time.deltaTime * fillSpeed;

                value = Mathf.Clamp(value, 0, meter.maxValue);
                meter.value = value;

                CheckFull();
                break;
            case BarState.COOLDOWN:
                coolDown += Time.deltaTime;

                CheckCoolDown();
                break;
        }
    }

    //Makes sure the meter doesnt flip with the player
    public void LateUpdate()
    {
        transform.rotation = lockedRotation;
    }

    private void SprintBeginEvent()
    {
        barState = BarState.CONSUMING;
    }

    private void SprintEndEvent()
    {
        if (barState == BarState.CONSUMING) {
            barState = BarState.FILLING;
        }
    }

    private void ShootBeginEvent()
    {
        transform.parent.gameObject.SetActive(false);
    }

    private void ShootEndEvent()
    {
        transform.parent.gameObject.SetActive(true);
    }

    private void CheckEmpty()
    {
        if(value == 0) {
            barState = BarState.COOLDOWN;
            actions.EnduranceDepleted();
        }
    }

    private void CheckFull()
    {
        if(value == meter.maxValue) {
            barState = BarState.STEADY;
        }
    }

    private void CheckCoolDown()
    {
        if(coolDown >= coolDownTime) {
            barState = BarState.FILLING;
            coolDown = 0;
        }
    }
}
