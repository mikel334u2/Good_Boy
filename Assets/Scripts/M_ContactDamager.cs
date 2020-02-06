using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_ContactDamager : MonoBehaviour
{
    public float damageAmmount;

    private void OnTriggerEnter(Collider other) 
    {
        if (other.TryGetComponent<M_Health>(out M_Health objHealth))
        {
            objHealth.modifyHealth(-damageAmmount);
        }
    }
}
