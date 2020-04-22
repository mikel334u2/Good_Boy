using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActivationTrigger : MonoBehaviour
{
    [SerializeField] private Animation cart;
    private bool hasTriggered;

    void Start()
    {
        hasTriggered = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!hasTriggered && CheckKeys())
            {
                cart.Play();
                hasTriggered = true;
            }
        }
    }

    private bool CheckKeys()
    {
        Quest quest = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInfo>().quests["The Keys to Success"];
        return quest != null && quest.Completed;
    }
}
