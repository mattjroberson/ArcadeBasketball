using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogicScript : MonoBehaviour
{
    private float targetPlaybackSpeed;
    private float slomoLerpValue;
    private float slomoLerpSpeed;
    private bool lerpingPlayback;

    private const float TIME_TO_LERP_PERCENT = .10f;

    public static GameLogicScript Instance;

    public void Awake()
    {
        Instance = this;

        //Set up some display variables
        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 0;

        lerpingPlayback = false;
    }

    public void Start()
    {
    }

    public void Update()
    { 
        if (lerpingPlayback) {
            float newTime = Mathf.Lerp(1, targetPlaybackSpeed, slomoLerpValue);
            slomoLerpValue += slomoLerpSpeed * Time.unscaledDeltaTime;
            SetPlaybackSpeed(newTime);

            //Stop lerping when completely lerped
            if (newTime == targetPlaybackSpeed) {
                lerpingPlayback = false;
            }
        }
    }

    public void SetPlaybackSpeedOnShot(float jumpTime, float meterTime)
    {
        slomoLerpValue = 0f;
        targetPlaybackSpeed = jumpTime / meterTime;
        slomoLerpSpeed = 1 / (jumpTime * TIME_TO_LERP_PERCENT);
        lerpingPlayback = true;
    }

    public void ClearPlaybackSpeed()
    {
        lerpingPlayback = false;
        SetPlaybackSpeed(1);
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
}
