using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Health : MonoBehaviour
{
    public float current;
    public float capacity;
    public bool respawn = true;
    private M_PlayerController controller = null;
    public Vector3 respawnPos;
    private Animator animator;
    private M_FadeOut fadeOut;
    public GameObject spawn;
    public float chanceToSpawn;

    private void Start() 
    {
        current = capacity;
        respawnPos = transform.position;
        TryGetComponent<M_PlayerController>(out controller);
        TryGetComponent<Animator>(out animator);
        TryGetComponent<M_FadeOut>(out fadeOut);
    }

    public void modifyHealth(float amount)
    {
        // TODO: spot for animation
        current += amount;
        if (current > capacity)
        {
            current = capacity;
        }
        else if (current <= 0 && !animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerDeath"))
        {
            animator.SetTrigger("Die");
            if (respawn)
            {
                // controller.grounded = true;
                controller.zeroMovement = true;
                // kill object after animation finishes
                Invoke("respawnObject", animator.GetCurrentAnimatorStateInfo(0).length);
                //respawnObject();
            } else
            {
                // kill object after animation finishes
                killObject();
                Invoke("killObject", animator.GetCurrentAnimatorStateInfo(0).length);
            }
        }
    }

    public void killObject()
    {
        // TODO: spot for animation
        SpawnObject();
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void respawnObject()
    {
        if (fadeOut != null)
            {
                fadeOut.RestartFade(false);
            }
        // TODO: Spot for animation
        if (controller != null)
        {
            Invoke("Reposition", 1.0f);
            Invoke("enableController", 2.5f);
        }
        
    }

    void Reposition()
    {
        transform.position = respawnPos;
        if (fadeOut != null)
            {
                fadeOut.RestartFade(true);
            }
        animator.SetTrigger("Respawn");
    }
    void enableController()
    {
        current = capacity;
        controller.zeroMovement = false;
    }
    void SpawnObject()
    {
        var chance = Random.value;
        if (chance <= chanceToSpawn)
        {
            Instantiate(spawn, transform.position + new Vector3(0,1,0), Quaternion.identity);
        }
    }
}
