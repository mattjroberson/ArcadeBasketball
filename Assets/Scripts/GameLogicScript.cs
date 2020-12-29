using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogicScript : MonoBehaviour
{
    private BallScript basketball;

    private float targetPlaybackSpeed;
    private float slomoLerpValue;
    private float slomoLerpSpeed;
    private bool lerpingPlayback;

    public void Awake()
    {
        //Set up some display variables
        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 0;

        lerpingPlayback = false;
        slomoLerpSpeed = .3f;
    }

    public void Start()
    {
        basketball = GameObject.Find("Basketball").GetComponent<BallScript>();
    }

    public void Update()
    { 
        if (lerpingPlayback) {
            float newTime = Mathf.Lerp(Time.timeScale, targetPlaybackSpeed, slomoLerpValue);
            slomoLerpValue += slomoLerpSpeed * Time.unscaledDeltaTime;
            SetPlaybackSpeed(newTime);

            //Stop lerping when completely lerped
            if (newTime == targetPlaybackSpeed) {
                Debug.Log("on");
                lerpingPlayback = false;
            }
        }
    }

    public void SetPlaybackSpeed(float speed, bool lerp)
    {
        if (lerp == true) {
            slomoLerpValue = 0f;
            targetPlaybackSpeed = speed;
        }
        else {
            SetPlaybackSpeed(speed);
        }

        lerpingPlayback = lerp;
    }

    private void SetPlaybackSpeed(float speed) {
        Time.timeScale = speed;
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }

    //Check to see if the player is going to make the shot
    public static bool CalculateIfMadeShot(float shotPercentage)
    {
        //Random value between 0 - 1
        float chance = Random.value;

        //If chance is within shotPercentage, return true
        if (chance <= shotPercentage) return true;
        else return false;
    }

    public BallScript GetBasketball() { return basketball; }
}
