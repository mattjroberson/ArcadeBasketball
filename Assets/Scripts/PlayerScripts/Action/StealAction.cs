using System.Collections;
using UnityEngine;

public class StealAction : IAction
{
    private PlayerScript player;
    private int stealAttempts;

    public StealAction(PlayerScript player)
    {

        this.player = player;
    }

    public void Start()
    {
        IncrementStealAttempts();

        //Do nothing if the defender fouled
        if (CheckForStealFoul() == true) return;

        //If probability calculated for a steal, steal ball
        if (Random.value <= player.Attributes.GetStealProbability())
        {
            BallEvents.Instance.BallStolen(player);
        }
    }

    private void IncrementStealAttempts()
    {
        if (stealAttempts == 0) player.StartCoroutine(ClearStealAttempts());
        stealAttempts++;
    }

    private IEnumerator ClearStealAttempts()
    {
        yield return new WaitForSeconds(player.Attributes.GetStealAttemptWindow());
        stealAttempts = 0;
    }

    private bool CheckForStealFoul()
    {
        if (stealAttempts > player.Attributes.GetStealAttemptLimit())
        {
            //If the probability results in a foul, return true
            if (Random.value <= player.Attributes.GetStealFoulProbability())
            {
                Debug.Log("Player Fouled");
                return true;
            }
        }

        return false;
    }
}
