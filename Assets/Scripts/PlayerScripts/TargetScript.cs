using UnityEngine;

public class TargetScript : MonoBehaviour
{
    public Vector2 PlayerPos => transform.position;
    public Vector2 FrontPointPos => frontPointTransform.position;

    [SerializeField] private Transform frontPointTransform;
}
