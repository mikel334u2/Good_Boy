using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Health : MonoBehaviour
{
    public float current;
    public float capacity;
    public bool respawn = true;
    private M_PlayerController controller;
    public Vector3 respawnPos;

    private void Start() 
    {
        current = capacity;
        respawnPos = transform.position;
        TryGetComponent<M_PlayerController>(out controller);
    }
    public void modifyHealth(float amount)
    {
        // TODO: spot for animation
        current += amount;
        if (current > capacity)
        {
            current = capacity;
        }
        else if (current <= 0)
        {
            if (respawn)
            {
                respawnObject();
            } else
            {
                killObject();
            }
        }
    }

    public void killObject()
    {
        // TODO: spot for animation
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void respawnObject()
    {
        // TODO: Spot for animation
        if (controller != null)
        {
            controller.grounded = true;
            controller.enabled = false;
            Invoke("Reposition", 1.5f);
            Invoke("enableController", 1.5f);
        }
        
    }

    void Reposition()
    {
        transform.position = respawnPos;
    }
    void enableController()
    {
        current = capacity;
        controller.enabled = true;
    }
}
