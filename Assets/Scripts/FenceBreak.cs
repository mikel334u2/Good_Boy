using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GIVE THE FENCE A COLLIDER
public class FenceBreak : MonoBehaviour
{
    private M_PlayerController playerController;

    private bool collided = false;
    private Vector3 velocity = new Vector3(0,0,0);

    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindGameObjectWithTag("Player").TryGetComponent<M_PlayerController>(out playerController);
    }

    void Update()
    {
        // Debug.Log(velocity);
        transform.Translate(velocity);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player" && playerController.sprint)
        {
            GetComponent<Collider>().enabled = false;
            velocity = playerController.velocity * Time.deltaTime;
        }
    }
}
