using UnityEngine;

public class UserIntel : IntelligenceType
{
    private InputController inputController;

    private bool jumpPressed;
    private bool sprintPressed;

    public UserIntel(PlayerScript player, ActionsScript actions) : base(player, actions) {
        inputController = GameObject.Find("InputController").GetComponent<InputController>();
    }

    public override void Wake() { 
        base.Wake();
        actions.events.onAiZoneTouched += AiZoneTouchedEvent;
    }

    public override void Sleep() { 
        base.Sleep();
        actions.events.onAiZoneTouched -= AiZoneTouchedEvent;
    }

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

    private void AiZoneTouchedEvent(string name)
    {
        if (player.States.HasBall) player.Team.IntelStates.BallHandlerPositionUpdated(name);
    }
}
