using UnityEngine;

public class IntelligenceContainer : MonoBehaviour
{
    public enum IntelligenceType { USER, OFFBALL_OFF, ONBALL_OFF, OFFBALL_DEF, ONBALL_DEF };

    public IntelligenceType intelType;

    private IntelligenceScript current;
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
                current = userIntelligence;
                break;
            case IntelligenceType.OFFBALL_OFF:
                current = offBallOffenseIntelligence;
                break;
            case IntelligenceType.ONBALL_OFF:
                current = onBallOffenseIntelligence;
                break;
            case IntelligenceType.OFFBALL_DEF:
                current = offBallDefenseIntelligence;
                break;
            case IntelligenceType.ONBALL_DEF:
                current = onBallDefenseIntelligence;
                break;
            default:
                Debug.LogWarning("Intelligence Type Not Found");
                break;
        }
    }
    
    //Returns the current Intelligence Script
    public IntelligenceScript Current() { return current; }

}
