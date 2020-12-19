using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntelligenceContainer : MonoBehaviour
{
    public enum IntelligenceType { USER, OFFBALL_OFF, ONBALL_OFF, OFFBALL_DEF, ONBALL_DEF };

    public IntelligenceType intelType;

    private IntelligenceScript intelligence;
    private UserIntelligence userIntelligence;
    private OffBallOffenseIntelligence offBallOffenseIntelligence;
    private OnBallOffenseIntelligence onBallOffenseIntelligence;
    private OffBallDefenseIntelligence offBallDefenseIntelligence;
    private OnBallDefenseIntelligence onBallDefenseIntelligence;

    public void Start()
    {
        userIntelligence = GetComponent<UserIntelligence>();
        offBallOffenseIntelligence = GetComponent<OffBallOffenseIntelligence>();
        onBallOffenseIntelligence = GetComponent<OnBallOffenseIntelligence>();
        offBallDefenseIntelligence = GetComponent<OffBallDefenseIntelligence>();
        onBallDefenseIntelligence = GetComponent<OnBallDefenseIntelligence>();

        SetIntelligenceType(intelType);
    }

    public void SetIntelligenceType(IntelligenceType type)
    {
        intelType = type;

        switch (type) {
            case IntelligenceType.USER:
                intelligence = userIntelligence;
                break;
            case IntelligenceType.OFFBALL_OFF:
                intelligence = offBallOffenseIntelligence;
                break;
            case IntelligenceType.ONBALL_OFF:
                intelligence = onBallOffenseIntelligence;
                break;
            case IntelligenceType.OFFBALL_DEF:
                intelligence = offBallDefenseIntelligence;
                break;
            case IntelligenceType.ONBALL_DEF:
                intelligence = onBallDefenseIntelligence;
                break;
            default:
                Debug.LogWarning("Intelligence Type Not Found");
                break;
        }
    }
    
    //Returns the current Intelligence Script
    public IntelligenceScript Current() { return intelligence; }

}
