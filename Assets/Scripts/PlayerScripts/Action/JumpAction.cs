
using System;

public class JumpAction : ILongAction
{
    private readonly ActionsScript actions;
    private readonly PlayerStateScript states;
    private bool isJumping;

    public JumpAction(ActionsScript actions, PlayerStateScript states)
    {
        this.actions = actions;
        this.states = states;
        isJumping = false;
    }

    public void Start()
    {
        throw new NotImplementedException();
    }

    public void Start(float scalar)
    {
        if (isJumping == false && states.IsFrozen == false)
        {
            isJumping = true;
            actions.events.JumpBegin(scalar);
        }
    }

    public void Stop()
    {
        actions.events.JumpStop();
    }

    public void End()
    {
        isJumping = false;
        actions.events.JumpEnd();
    }

    public bool IsActive()
    {
        return isJumping;
    }
}
