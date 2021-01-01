
using UnityEngine;

public class ShootAction : IAction
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

        actions.GetJumpAction().Start(.5f);
        actions.events.ShootBegin();
    }

    public void Stop()
    {
        if(isShooting == true)
        {
            isShooting = false;
            actions.events.ShootEnd();

            if(playerWalked == false)
            {
                actions.CompleteShotProcess();
            }

        }
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

    //TODO Reimplement
    //gameLogic.SetPlaybackSpeed(.3f, true);

}
