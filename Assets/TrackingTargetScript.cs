using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingTargetScript : MonoBehaviour
{
    public TargetScript Target { get; private set; }
    
    [SerializeField] private PlayerScript targetPlayer;

    private void Awake()
    {
        Target = GetComponentInChildren<TargetScript>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        transform.position = targetPlayer.transform.position;
    }
}
