

public class SprintAction : ILongAction
{
    private ActionsScript actions;
    private bool isSprinting;

    public SprintAction(ActionsScript actions)
    {
        this.actions = actions;
        isSprinting = false;
    }

    public void Start()
    {
        isSprinting = true;
        actions.events.SprintBegin();
    }

    public void Stop()
    {
        isSprinting = false;
        actions.events.SprintEnd();
    }

    public bool IsActive()
    {
        return isSprinting;
    }

}
