using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActivationTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent actions;
    [SerializeField] private bool isPermanent = true;
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            actions.Invoke();
            if (isPermanent)
                hasTriggered = true;
        }
    }
}
