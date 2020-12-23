using System;

public class ActionEvents
{
    public event Action onJumpBegin;
    public void JumpBegin()
    {
        onJumpBegin?.Invoke();
    }

    public event Action onJumpEnd;
    public void JumpEnd()
    {
        onJumpEnd?.Invoke();
    }

    public event Action onSprintBegin;
    public void SprintBegin()
    {
        onSprintBegin?.Invoke();
    }

    public event Action onSprintEnd;
    public void SprintEnd()
    {
        onSprintEnd?.Invoke();
    }

    public event Action onDunkBegin;
    public void DunkBegin()
    {
        onDunkBegin?.Invoke();
    }

    public event Action onDunkEnd;
    public void DunkEnd()
    {
        onDunkEnd?.Invoke();
    }

    public event Action onShootBegin;
    public void ShootBegin()
    {
        onShootBegin?.Invoke();
    }

    public event Action onShootEnd;
    public void ShootEnd()
    {
        onShootEnd?.Invoke();
    }

    public event Action onPassBegin;
    public void PassBegin()
    {
        onPassBegin?.Invoke();
    }

    public event Action onStealBegin;
    public void StealBegin()
    {
        onStealBegin?.Invoke();
    }

    public event Action onWalkViolation;
    public void WalkViolation()
    {
        onWalkViolation?.Invoke();
    }
}
