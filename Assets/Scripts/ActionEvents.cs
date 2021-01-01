using System;
using UnityEngine;

public class ActionEvents
{
    public event Action<float> onJumpBegin;
    public void JumpBegin(float scalar)
    {
        onJumpBegin?.Invoke(scalar);
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

    public event Action onShotInit;
    public void ShotInit()
    {
        onShotInit?.Invoke();
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

    public event Action<Transform> onPassBegin;
    public void PassBegin(Transform target)
    {
        onPassBegin?.Invoke(target);
    }

    public event Action onSwipeBegin;
    public void SwipeBegin()
    {
        onSwipeBegin?.Invoke();
    }

    public event Action onWalkViolation;
    public void WalkViolation()
    {
        onWalkViolation?.Invoke();
    }

    public event Action onEnduranceDepleted;
    public void EnduranceDepleted()
    {
        onEnduranceDepleted?.Invoke();
    }
}
