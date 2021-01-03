public class DunkAction : ILongAction
{
    private ActionsScript actions;
    private bool isDunking;

    public DunkAction(ActionsScript actions)
    {
        this.actions = actions;
        isDunking = false;
    }

    public void Start()
    {
        isDunking = true;
        actions.events.DunkBegin();
    }

    public void Stop()
    {
        isDunking = false;
        actions.events.DunkEnd();
    }
    public bool IsActive()
    {
        return isDunking;
    }
}
