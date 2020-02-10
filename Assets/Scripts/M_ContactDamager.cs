using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_ContactDamager : MonoBehaviour
{
    public float damageAmmount;
    public bool onlyForPlayer = false;
    public bool destroyWhenDamaged = false;

    private void OnTriggerEnter(Collider other) 
    {
        if (other.TryGetComponent<M_Health>(out M_Health objHealth))
        {
            Debug.Log(other.name);
            foreach(var h in GetComponentsInParent<M_Health>())
            {
                if (objHealth.gameObject == h.gameObject || (onlyForPlayer && !objHealth.gameObject.tag.Equals("Player")))
                {
                    return;
                }
            }
            
            if (objHealth.current >= 0)
            {
                objHealth.modifyHealth(-damageAmmount);
                Debug.Log(other.name);
            }

            if (destroyWhenDamaged)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
            
        }
        if(other.tag.Equals("Glock"))
        {
            if (objHealth.current >= 0)
            {
                objHealth.modifyHealth(-damageAmmount);
            }
        }
    }
}
