
public class JumpAction : IAction
{
    private readonly ActionsScript actions;
    private bool isJumping;

    public JumpAction(ActionsScript actions)
    {
        this.actions = actions;
        isJumping = false;
    }

    public void Start()
    {
        if (isJumping == false)
        {
            isJumping = true;
            actions.events.JumpBegin();
        }
    }
    public void Stop()
    {
        isJumping = false;
        actions.events.JumpEnd();
    }

    public bool IsActive()
    {
        return isJumping;
    }
}
