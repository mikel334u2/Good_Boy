using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_ScoreItem : MonoBehaviour
{
    public M_Score score;
    public int value = 1;

    private void OnTriggerEnter(Collider other) 
    {
        if (other.TryGetComponent<CharacterController>(out var controller))
        {
            score.ModifyScore(value);
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
