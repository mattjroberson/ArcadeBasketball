using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserIntelligence : IntelligenceScript
{
    private InputController inputController;

    private bool jumpPressed;
    private bool sprintPressed;

    public void Awake()
    {
        moveDirection = Vector2.zero;
    }

    public void Start()
    {
        inputController = GameObject.Find("InputController").GetComponent<InputController>();
        player = transform.parent.GetComponentInParent<PlayerScript>();
        actions = transform.parent.GetComponentInParent<ActionsScript>();
    }

    public override void FixedUpdateIntelligence() {}

    public override void UpdateIntelligence()
    {
        moveDirection = inputController.GetInputDirection();

        UpdateJumpLogic();
        UpdateSprintLogic();
        UpdateSwitchLogic();
        UpdateStealLogic();
    }

    private void UpdateJumpLogic()
    {
        if (inputController.GetJumpPressed() == true) {
            if (jumpPressed == false) {
                HandleJumpOrShoot(true);
                jumpPressed = true;

            }
        }
        else {
            if (jumpPressed == true) {
                HandleJumpOrShoot(false);
                jumpPressed = false;
            }
        }
    }

    private void UpdateSprintLogic()
    {
        if (inputController.GetSprintPressed() == true) {
            if (sprintPressed == false) {
                actions.StartSprinting();
                sprintPressed = true;
            }
        }
        else {
            if(sprintPressed == true) {
                actions.StopSprinting();
                sprintPressed = false;
            }
        }
    }
    
    private void UpdateSwitchLogic() {
        if (inputController.GetSwitchPressed() == true) {
            if (player.GetHasBall() == true) {
                inputController.SetSwitchPressed(false);
                actions.StartPassing();
            }
        }
    }

    private void UpdateStealLogic()
    {
        if (inputController.GetStealPressed() == true) {
            inputController.SetStealPressed(false);
            if (player.IsOffense() == false) {
                actions.AttemptSteal();
            }
        }
    }

    private void HandleJumpOrShoot(bool isStart)
    {
        //If the player is on offense, shoot
        if(player.GetHasBall() == true) {
            if (isStart == true) actions.HandleShotType();
            else actions.ReleaseShot();
        }
        //Otherwise jump
        else {
            if (isStart == true) actions.StartJumping();
        }
    }
}
