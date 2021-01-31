using System.Collections;
using UnityEngine;

public class AnimatorScript : MonoBehaviour
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite jumpingSprite;
    [SerializeField] private Sprite reachingSprite;
    [SerializeField] private Sprite dunkingSprite;

    public enum SpriteType { DEFAULT, JUMPING, REACHING, DUNKING }

    private SpriteRenderer spriteRenderer;
    private ActionsScript actions;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        actions = GetComponent<ActionsScript>();

        actions.events.onJumpBegin += JumpBeginEvent;
        actions.events.onJumpEnd += JumpEndEvent;
        actions.events.onDunkBegin += DunkBeginEvent;
        actions.events.onDunkEnd += DunkEndEvent;
        actions.events.onPassBegin += PassEvent;
        actions.events.onSwipeBegin += SwipeEvent;

        SetSprite(SpriteType.DEFAULT);
    }

    void Update()
    {
        
    }

    public void SetSprite(SpriteType type)
    {
        switch (type)
        {
            case SpriteType.DEFAULT:
                spriteRenderer.sprite = defaultSprite;
                break;
            case SpriteType.JUMPING:
                spriteRenderer.sprite = jumpingSprite;
                break;
            case SpriteType.REACHING:
                spriteRenderer.sprite = reachingSprite;
                StartCoroutine(PutHandsDown());
                break;
            case SpriteType.DUNKING:
                spriteRenderer.sprite = dunkingSprite;
                break;
        }
    }

    private IEnumerator PutHandsDown()
    {
        yield return new WaitForSeconds(.25f);
        SetSprite(SpriteType.DEFAULT);
    }

    private void JumpBeginEvent(float scalar)
    {
        SetSprite(SpriteType.JUMPING);
    }

    private void JumpEndEvent()
    {
        SetSprite(SpriteType.DEFAULT);
    }

    private void DunkBeginEvent()
    {
        SetSprite(SpriteType.DUNKING);
    }

    private void DunkEndEvent()
    {
        SetSprite(SpriteType.DEFAULT);
    }

    private void PassEvent(Transform target)
    {
        SetSprite(SpriteType.REACHING);
    }

    private void SwipeEvent()
    {
        SetSprite(SpriteType.REACHING);
    }
}
