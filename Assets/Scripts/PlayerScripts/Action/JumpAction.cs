
public class JumpAction
{
    private readonly ActionsScript actions;
    private bool isJumping;

    public JumpAction(ActionsScript actions)
    {
        this.actions = actions;
        isJumping = false;
    }

    public void Start(float scalar)
    {
        if (isJumping == false)
        {
            isJumping = true;
            actions.events.JumpBegin(scalar);
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
