using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private Vector2 inputDirection;

    private bool jumpPressed;
    private bool sprintPressed;
    private bool switchPressed;
    private bool stealPressed;

    public void Awake()
    { 
        inputDirection = Vector2.zero;
    }

    void Update()
    {
        //Movement for Keyboard
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) {
            inputDirection.x = Input.GetAxisRaw("Horizontal");
            inputDirection.y = Input.GetAxisRaw("Vertical");

            //Shoot / Jump / Block
            if (Input.GetButtonDown("JumpButton")) {
                jumpPressed = true;
            }
            else if (Input.GetButtonUp("JumpButton")) {
                jumpPressed = false;
            }
            //Sprint
            else if (Input.GetButtonDown("SprintButton")) {
                sprintPressed = true;
            }
            else if (Input.GetButtonUp("SprintButton")) {
                sprintPressed = false;
            }
            //Pass / Switch
            else if (Input.GetButtonDown("SwitchButton")) {
                switchPressed = true;
            }
            //Steal
            else if (Input.GetButtonDown("StealButton")) {
                stealPressed = true;
            }
        }
        //Movement for Mobile
        else {
            
        }
    }

    public Vector2 GetInputDirection() { return inputDirection; }

    public bool GetJumpPressed() { return jumpPressed; }

    public bool GetSprintPressed() { return sprintPressed; }

    public bool GetSwitchPressed() { return switchPressed; }

    public void SetSwitchPressed(bool newSwitchPressed) { switchPressed = newSwitchPressed; }

    public bool GetStealPressed() { return stealPressed; }

    public void SetStealPressed(bool newStealPressed) { stealPressed = newStealPressed; }
}
