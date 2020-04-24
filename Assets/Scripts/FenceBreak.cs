using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GIVE THE FENCE A COLLIDER
public class FenceBreak : MonoBehaviour
{
    private M_PlayerController playerController;
    private Vector3 velocity = new Vector3(0,0,0);

    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindGameObjectWithTag("Player").TryGetComponent<M_PlayerController>(out playerController);
    }

    void Update()
    {
        // Debug.Log(velocity);
        transform.Translate(velocity * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && playerController.isSprinting)
        {
            foreach (Collider c in GetComponents<Collider>())
            {
                c.enabled = false;
            }
            velocity = playerController.velocity;
            // Debug.Log("Player controller velocity: " + playerController.velocity);
            if (velocity.y <= 0)
                velocity.y = -velocity.y;
            Destroy(gameObject, 15f);
        }
    }
}
