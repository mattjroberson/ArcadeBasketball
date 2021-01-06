using UnityEngine;

public class FootScript : MonoBehaviour
{
    private ActionsScript actions;

    private void Start()
    {
        actions = GetComponentInParent<ActionsScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("basketball")) actions.FootTouchBall();
    }
}
