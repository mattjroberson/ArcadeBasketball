using UnityEngine;

public class UserIntelligence : IntelligenceScript
{
    private InputController inputController;

    private bool jumpPressed;
    private bool sprintPressed;

    public UserIntelligence(IntelligenceContainer intelligence) : base(intelligence) {
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
            if (playerStates.HasBall) {
                actions.StartPassing();
            }
            else
            {
                Debug.Log("test");
            }
        }
    }

    private void UpdateStealLogic()
    {
        if (inputController.ReadStealPressed()) {
            if (playerStates.IsOffense == false) {
                actions.ReachForSteal();
            }
        }
    }

    private void DecideJumpOrShoot(bool isStart)
    {
        if(playerStates.HasBall) {
            if (isStart == true) actions.InitializeShot();
            else actions.GetShootAction().Stop();
        }
        else {
            if (isStart == true) actions.GetJumpAction().Start(1);
            //TODO implement jump exit, right now floaty bug happens
            //else actions.GetJumpAction().Stop();
        }
    }
}
