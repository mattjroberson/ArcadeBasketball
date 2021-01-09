using UnityEngine;

public class PlayerScript : MonoBehaviour, ISortableSprite
{   
    [SerializeField] private PlayerScript teammate;
    public PlayerScript Teammate => teammate;

    [SerializeField] private PlayerScript defender;
    public PlayerScript Defender => defender;

    [SerializeField] private AttributeSO attributes;
    public AttributeSO Attributes => attributes;

    private PlayerStates playerStates;
    public PlayerStates States => playerStates;

    private FrontPointScript frontPoint;
    public FrontPointScript FrontPoint => frontPoint;

    private TeamScript teamScript;
    public TeamScript Team => teamScript;

    private Transform hands;
    public Transform Hands => hands.transform;

    private SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer => spriteRenderer;

    public float SortPosition => FrontPoint.FloorPosition.y;

    public void Awake()
    {
        playerStates = new PlayerStates(this);
        teamScript = GetComponentInParent<TeamScript>();
        attributes.Init();
    }

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        hands = GetComponentInChildren<HandScript>().transform;
        frontPoint = GetComponentInChildren<FrontPointScript>();
    }

}
