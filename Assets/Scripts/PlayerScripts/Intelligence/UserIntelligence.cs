using UnityEngine;

public class UserIntelligence : IntelligenceScript
{
    private InputController inputController;

    private bool jumpPressed;
    private bool sprintPressed;

    public override void Start()
    {
        base.Start();
        inputController = GameObject.Find("InputController").GetComponent<InputController>();
    }

    public override void FixedUpdateIntelligence() {}

    public override void UpdateIntelligence()
    {
        SetMoveDirection(inputController.GetInputDirection());

        UpdateJumpLogic();
        UpdateSprintLogic();
        UpdateSwitchLogic();
        UpdateStealLogic();
    }

    private void UpdateJumpLogic()
    {
        if (jumpPressed == inputController.GetJumpPressed()) return;
        jumpPressed = inputController.GetJumpPressed();

        DecideJumpOrShoot(inputController.GetJumpPressed());
    }

    private void UpdateSprintLogic()
    {
        if (inputController.GetSprintPressed() == sprintPressed) return;
        sprintPressed = inputController.GetSprintPressed();

        if (sprintPressed) actions.GetSprintAction().Start();
        else actions.GetSprintAction().Stop();
    }
    
    private void UpdateSwitchLogic() {
        if (inputController.ReadSwitchPressed()) {
            if (playerStates.GetHasBall() == true) {
                actions.StartPassing();
            }
        }
    }

    private void UpdateStealLogic()
    {
        if (inputController.ReadStealPressed()) {
            if (playerStates.IsOffense() == false) {
                actions.ReachForSteal();
            }
        }
    }

    private void DecideJumpOrShoot(bool isStart)
    {
        if(playerStates.GetHasBall() == true) {
            if (isStart == true) actions.InitializeShot();
            else actions.GetShootAction().Stop();
        }
        else {
            if (isStart == true) actions.GetJumpAction().Start();
            //TODO implement jump exit, right now floaty bug happens
            //else actions.GetJumpAction().Stop();
        }
    }
}
