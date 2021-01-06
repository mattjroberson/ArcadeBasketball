
using UnityEngine;

public class ShootAction : ILongAction
{
    private readonly ActionsScript actions;
    private bool isShooting;
    private bool playerWalked;

    public ShootAction(ActionsScript actions)
    {
        this.actions = actions;
        isShooting = false;
        playerWalked = false;
    }

    public void Start()
    {
        isShooting = true;
        playerWalked = false;

        actions.GetJumpAction().Start(1f);
        actions.events.ShootBegin();
        GameLogicScript.Instance.SetPlaybackSpeedOnShot();
    }

    public void Stop()
    {
        if (isShooting == false) return;

        isShooting = false;
        actions.events.ShootEnd();

        if(playerWalked == false)
        {
            actions.CompleteJumpShotProcess();
        }
        
        GameLogicScript.Instance.ClearPlaybackSpeed();
    }

    public bool IsActive()
    {
        return isShooting;
    }

    public void PlayerWalked()
    {
        Debug.Log("walked!");
        playerWalked = true;
        Stop();
    }
}
