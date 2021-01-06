using UnityEngine;

public class IntelligenceContainer : MonoBehaviour
{
    public enum IntelligenceType { USER, OFFBALL_OFF, ONBALL_OFF, OFFBALL_DEF, ONBALL_DEF };
    [SerializeField] private IntelligenceType intelType;

    private ActionsScript actions;
    public ActionsScript Actions => actions;
   
    private PlayerStateScript playerStates;
    public PlayerStateScript PlayerStates => playerStates;

    private PlayerScript player;

    private IntelligenceScript current;
    private IntelligenceScript userIntel;
    private IntelligenceScript offBallOffIntel;
    private IntelligenceScript onBallOffIntel;
    private IntelligenceScript offBallDefIntel;
    private IntelligenceScript onBallDefIntel;

    public void Start()
    {
        player = GetComponentInParent<PlayerScript>();
        actions = GetComponentInParent<ActionsScript>();
        playerStates = GetComponentInParent<PlayerStateScript>();

        userIntel = new UserIntelligence(this);
        offBallOffIntel = new OffBallOffenseIntelligence(this);
        onBallOffIntel = new OnBallOffenseIntelligence(this);
        offBallDefIntel = new OffBallDefenseIntelligence(this);
        onBallDefIntel = new OnBallDefenseIntelligence(this);

        SetIntelligenceType(intelType);

        GameEvents.Instance.onPossessionChange += PossessionChangeEvent;
        GameEvents.Instance.onUserPlayerSwitch += UserPlayerSwitchEvent;
    }

    public void Update()
    {
        current.UpdateIntelligence();
    }

    private void SetIntelligenceType(IntelligenceType type)
    {
        intelType = type;

        switch (type) {
            case IntelligenceType.USER:
                current = userIntel;
                break;
            case IntelligenceType.OFFBALL_OFF:
                current = offBallOffIntel;
                break;
            case IntelligenceType.ONBALL_OFF:
                current = onBallOffIntel;
                break;
            case IntelligenceType.OFFBALL_DEF:
                current = offBallDefIntel;
                break;
            case IntelligenceType.ONBALL_DEF:
                current = onBallDefIntel;
                break;
            default:
                Debug.LogWarning("Intelligence Type Not Found");
                break;
        }
    }

    private void PossessionChangeEvent(PlayerScript newBallHandler)
    {
        if (player.Team.UserControlled)
        {
            if (newBallHandler == player)
            {
                SetIntelligenceType(IntelligenceType.USER);
            }
            else if (newBallHandler == player.Teammate)
            {
                SetIntelligenceType(IntelligenceType.OFFBALL_OFF);
            }
            else if (newBallHandler == player.Defender)
            {
                if (intelType != IntelligenceType.USER) SetIntelligenceType(IntelligenceType.ONBALL_DEF);
            }
            else
            {
                if (intelType != IntelligenceType.USER) SetIntelligenceType(IntelligenceType.OFFBALL_DEF);
            }
        }
        else
        {
            if (newBallHandler == player) SetIntelligenceType(IntelligenceType.ONBALL_OFF);
            else if (newBallHandler == player.Teammate) SetIntelligenceType(IntelligenceType.OFFBALL_OFF);
            else if (newBallHandler == player.Defender) SetIntelligenceType(IntelligenceType.ONBALL_DEF);
            else SetIntelligenceType(IntelligenceType.OFFBALL_DEF);
        }
    }

    private void UserPlayerSwitchEvent(PlayerScript oldUser)
    {
        if (oldUser == player)
        {
            IntelligenceType type = (intelType != IntelligenceType.ONBALL_DEF) ?
                IntelligenceType.ONBALL_DEF :
                IntelligenceType.ONBALL_OFF;

            SetIntelligenceType(type);
        }
        else if (oldUser == player.Teammate)
        {
            SetIntelligenceType(IntelligenceType.USER);
        }
    }
}
