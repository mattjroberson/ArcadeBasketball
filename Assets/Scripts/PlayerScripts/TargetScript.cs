using UnityEngine;

public class TargetScript : MonoBehaviour
{
    public Vector2 PlayerPos => transform.position;
    public Vector2 FrontPointPos => frontPointTransform.position;
    public Vector2 FeetPos => new Vector2(transform.position.x, frontPointTransform.position.y);

    [SerializeField] private Transform frontPointTransform;
}


