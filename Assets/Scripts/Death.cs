using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            IEnumerator coroutine = other.gameObject.GetComponent<PlayerInfo>().Respawn();
            StartCoroutine(coroutine);
        }
    }
}
