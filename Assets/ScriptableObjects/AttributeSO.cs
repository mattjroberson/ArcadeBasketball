using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName ="New Attributes", menuName ="Attributes")]
public class AttributeSO : ScriptableObject
{
    //Attributes
    [SerializeField] private int speedAttr = 5;
    [SerializeField] private int jumpAttr = 5;
    [SerializeField] private int endurAttr = 5;
    [SerializeField] private int defenseAttr = 5;

    private Dictionary<string, int> shootingAttributes;
    private Dictionary<string, float> shootingPercentages;

    private float maxSpeed;
    private float sprintBonus;
    private float maxJump;
    private float maxEndur;
    private float stealProb;

    private float stealFoulProbability;

    private int stealAttemptLimit;
    private float stealAttemptWindow;

    private float shotMeterSpeed;

    public void InitializeAttributes()
    {

        //TODO Make this stuff more transparent / less hard coded
        //Make sure the attributes are in a valid range
        speedAttr = Mathf.Clamp(speedAttr, 0, 25);
        jumpAttr = Mathf.Clamp(jumpAttr, 0, 25);
        endurAttr = Mathf.Clamp(endurAttr, 0, 25);
        defenseAttr = Mathf.Clamp(defenseAttr, 0, 25);

        //Shooting attributes for each zone on the court
        shootingAttributes = new Dictionary<string, int>();

        shootingAttributes.Add("paint", 10);
        shootingAttributes.Add("top_three", 10);
        shootingAttributes.Add("bottom_three", 10);
        shootingAttributes.Add("top_jumper", 10);
        shootingAttributes.Add("bottom_jumper", 10);
        shootingAttributes.Add("long_three", 10);
        shootingAttributes.Add("backcourt", 10);

        shootingPercentages = CalculateShootingPercentages();

        //Calculate the variables dependent on attributes
        maxSpeed = 1.4f + ((speedAttr) * .2f);
        sprintBonus = 2f;
        maxJump = 3.5f + ((jumpAttr) * .04f);
        maxEndur = 1.25f + ((endurAttr) * .15f);
        stealProb = (5 + (defenseAttr)) / 100f;

        stealFoulProbability = 1f;

        stealAttemptLimit = 3;
        stealAttemptWindow = 3f;

        shotMeterSpeed = .8f;
    }

    //Calculates the shooting percentages based on the corresponding attributes
    private Dictionary<string, float> CalculateShootingPercentages()
    {
        Dictionary<string, float> shootingPercentages = new Dictionary<string, float>();

        foreach(KeyValuePair<string, int> entry in shootingAttributes) {
            shootingPercentages.Add(entry.Key, .5f);
        }

        return shootingPercentages;
    }

    public float GetShotPercentage(string zoneName) { return shootingPercentages[zoneName]; }
    
    public float GetStealProbability() { return stealProb; }

    public float GetStealFoulProbability() { return stealFoulProbability; }
     
    public float GetMaxJump() { return maxJump; }

    public float GetMaxSpeed() { return maxSpeed; }

    public float GetMaxEndurance() { return maxEndur; }

    public float GetShotMeterSpeed() { return shotMeterSpeed; }

    public float GetSprintBonus() { return sprintBonus; }

    public int GetStealAttemptLimit() { return stealAttemptLimit; }

    public float GetStealAttemptWindow() { return stealAttemptWindow; }

}
