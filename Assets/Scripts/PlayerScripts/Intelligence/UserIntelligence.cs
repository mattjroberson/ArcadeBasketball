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
            if (player.GetHasBall() == true) {
                actions.StartPassing();
            }
        }
    }

    private void UpdateStealLogic()
    {
        if (inputController.ReadStealPressed()) {
            if (player.IsOffense() == false) {
                actions.AttemptSteal();
            }
        }
    }

    private void DecideJumpOrShoot(bool isStart)
    {
        if(player.GetHasBall() == true) {
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
