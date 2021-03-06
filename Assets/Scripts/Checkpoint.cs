﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Attach this to your checkpoints. Checkpoints should have a collider 2D set to trigger.
    // If you want to make a sprite animate on activating the checkpoint, let me know! It shouldn't be too hard to program.
    private GameObject respawn;
    private bool activated = false;
    public bool oneTimeUse = false;
	
	void Start () {
        respawn = GameObject.FindGameObjectWithTag("Respawn");
	}

    private void OnTriggerEnter(Collider collision)
    {
        if (!activated && collision.CompareTag("Player"))
        {
            respawn.transform.position = transform.position;
            if (oneTimeUse)
                activated = true;
        }
    }
}
