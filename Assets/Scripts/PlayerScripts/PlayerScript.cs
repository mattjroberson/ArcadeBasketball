using UnityEngine;

public class PlayerScript : MonoBehaviour, ISortableSprite
{
    [SerializeField] private AttributeSO attributes;
    public AttributeSO Attributes => attributes;
    
    [SerializeField] private PlayerScript teammate;
    public PlayerScript Teammate => teammate;

    [SerializeField] private PlayerScript defender;
    public PlayerScript Defender => defender;

    private TeamScript teamScript;
    public TeamScript Team => teamScript;

    private HandScript hands;
    public Transform Hands => hands.transform;

    private FrontPointScript frontPoint;
    public FrontPointScript FrontPoint => frontPoint;

    private SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer => spriteRenderer;

    private PlayerStateScript playerStates;
    public PlayerStateScript States => playerStates;

    public float SortPosition => FrontPoint.FloorPosition.y;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        teamScript = GetComponentInParent<TeamScript>();
        playerStates = GetComponent<PlayerStateScript>();

        hands = GetComponentInChildren<HandScript>();
        frontPoint = GetComponentInChildren<FrontPointScript>();

        attributes.InitializeAttributes();
    }

}
