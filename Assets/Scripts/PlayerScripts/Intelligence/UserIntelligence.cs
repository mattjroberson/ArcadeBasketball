using UnityEngine;

public class UserIntelligence : IntelligenceScript
{
    private InputController inputController;

    private bool jumpPressed;
    private bool sprintPressed;

    public UserIntelligence(PlayerScript player, ActionsScript actions) : base(player, actions) {
        inputController = GameObject.Find("InputController").GetComponent<InputController>();
    }

    public override void FixedUpdateIntelligence() {}

    public override void Wake() {}

    public override void UpdateIntelligence()
    {
        player.States.CurrentMoveDirection = inputController.GetInputDirection();

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
            if (player.States.HasBall) {
                actions.StartPassing();
            }
            else
            {
                actions.SwitchPlayer();
            }
        }
    }

    private void UpdateStealLogic()
    {
        if (inputController.ReadStealPressed()) {
            if (player.States.IsOffense == false) {
                actions.ReachForSteal();
            }
        }
    }

    private void DecideJumpOrShoot(bool isStart)
    {
        if(player.States.HasBall) {
            if (isStart == true) actions.InitializeShot();
            else actions.GetShootAction().Stop();
        }
        else {
            if (isStart == true) actions.GetJumpAction().Start(1);
            else actions.GetJumpAction().Stop();
        }
    }
}
