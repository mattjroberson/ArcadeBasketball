using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName ="New Attributes", menuName ="Attributes")]
public class AttributeSO : ScriptableObject
{
    //Attributes
    [SerializeField] private Attribute speedAttr = new Attribute();
    [SerializeField] private Attribute shotAttr = new Attribute();
    [SerializeField] private Attribute jumpAttr = new Attribute();
    [SerializeField] private Attribute endurAttr = new Attribute();
    [SerializeField] private Attribute defenseAttr = new Attribute();

    //Properties
    public Property MaxSpeed { get; private set; }
    public Property SprintBonus { get; private set; }
    public Property MaxJump { get; private set; }
    public Property MaxEndur { get; private set; }

    public Property StealProb { get; private set; }
    public Property StealFoulProb { get; private set; }
    public Property StealAttemptLimit { get; private set; }
    public Property StealAttemptWindow { get; private set; }

    public Property ShotMeterSpeed { get; private set; }
    private Dictionary<string, Property> shootingPercentages;

    public void Init ()
    {
        shootingPercentages = new Dictionary<string, Property> {
            //Shooting properties for each zone on the courts
            { "paint", new Property(shotAttr, .5f, .5f) },
            { "top_three", new Property(shotAttr, .5f, .5f) },
            { "bottom_three", new Property(shotAttr, .5f, .5f) },
            { "top_jumper", new Property(shotAttr, .5f, .5f) },
            { "bottom_jumper", new Property(shotAttr, .5f, .5f) },
            { "long_three", new Property(shotAttr, .5f, .5f) },
            { "backcourt", new Property(shotAttr, .5f, .5f) }
        };

        ShotMeterSpeed = new Property(shotAttr, 1.3f, 1.3f);

        MaxSpeed = new Property(speedAttr, 1.4f, 5f);
        SprintBonus = new Property(speedAttr, 2f, 2f);
        MaxEndur = new Property(endurAttr, 1.25f, 5f);
        MaxJump = new Property(jumpAttr, 3.5f, 4.5f);

        StealProb = new Property(defenseAttr, .05f, .30f);
        StealFoulProb = new Property(defenseAttr, 1f, 1f);
        StealAttemptLimit = new Property(defenseAttr, 3f, 3f);
        StealAttemptWindow = new Property(defenseAttr, 3f, 3f);
    }

    public float GetShotPercentage(string zoneName) { return shootingPercentages[zoneName].Value; }

    private void OnValidate()
    {
        speedAttr.SetValue();
        shotAttr.SetValue();
        jumpAttr.SetValue();
        endurAttr.SetValue();
        defenseAttr.SetValue();
    }
}
