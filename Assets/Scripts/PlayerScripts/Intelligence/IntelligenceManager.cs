using UnityEngine;

public class IntelligenceManager : MonoBehaviour
{
    public enum IntelligenceType { USER, OFFBALL_OFF, ONBALL_OFF, OFFBALL_DEF, ONBALL_DEF };
    [SerializeField] private IntelligenceType intelType;

    private ActionsScript actions;
    private PlayerScript player;

    //TODO Make sure we want to do this global tag
    private global::IntelligenceType current;
    private global::IntelligenceType userIntel;
    private global::IntelligenceType offBallOffIntel;
    private global::IntelligenceType onBallOffIntel;
    private global::IntelligenceType offBallDefIntel;
    private global::IntelligenceType onBallDefIntel;

    public void Start()
    {
        player = GetComponentInParent<PlayerScript>();
        actions = GetComponentInParent<ActionsScript>();

        userIntel = new UserIntel(player, actions);
        offBallOffIntel = new OffBallOffenseIntel(player, actions);
        onBallOffIntel = new OnBallOffenseIntel(player, actions);
        offBallDefIntel = new OffBallDefenseIntel(player, actions);
        onBallDefIntel = new OnBallDefenseIntel(player, actions);

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
        current?.Sleep();
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

        current.Wake();
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
