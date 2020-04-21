using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spawns an NPC to the location of this object when player enters trigger
public class SpawnNPC : MonoBehaviour
{
    public Transform NPCObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            NPCObject.position = transform.position;
        }
    }
}
