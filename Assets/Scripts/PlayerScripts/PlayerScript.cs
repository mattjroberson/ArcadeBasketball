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

    private TeamScript teamScript;
    public TeamScript Team => teamScript;

    private Transform handsTransform;
    public Transform HandsTransform => handsTransform;

    private Transform frontPointTransform;
    public Transform FrontPointTransform => frontPointTransform;

    private SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer => spriteRenderer;

    public float SortPosition => States.FloorPosition.y;

    public void Awake()
    {
        playerStates = new PlayerStates(this);
        teamScript = GetComponentInParent<TeamScript>();
        attributes.Init();
    }

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        handsTransform = GetComponentInChildren<HandScript>().transform;
        frontPointTransform = GetComponentInChildren<FrontPointScript>().transform;
    }

}
