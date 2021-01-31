
using UnityEngine;

public class ShootAction : ILongAction
{
    private readonly ActionsScript actions;
    private readonly PlayerScript player;
    private bool isShooting;
    private bool playerWalked;

    private float jumpScalar = 1f;

    public ShootAction(ActionsScript actions, PlayerScript player)
    {
        this.actions = actions;
        this.player = player;
        isShooting = false;
        playerWalked = false;
    }

    public void Start()
    {
        isShooting = true;
        playerWalked = false;

        actions.GetJumpAction().Start(jumpScalar);
        actions.events.ShootBegin();

        Vector2 slowMoVars = GetSloMoVariables();
        GameLogicScript.Instance.SetPlaybackSpeedOnShot(slowMoVars.x, slowMoVars.y);
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

    private Vector2 GetSloMoVariables()
    {
        float jumpTime = (2 * (player.Attributes.MaxJump.Value * jumpScalar)) / -Physics2D.gravity.y;
        float meterTime = player.Attributes.ShotMeterSpeed.Value;
        return new Vector2(jumpTime, meterTime);
    }
}
