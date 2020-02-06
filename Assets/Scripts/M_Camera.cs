using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Camera : MonoBehaviour
{
    public Transform player;
    public float playerHeight = 1;
    float heading = 0;
    public float tilt = 15;
    public float minTilt = -80;
    public float maxTilt = 80;
    public float camDist = 10;
    public float sensitivity = 180;
    public bool isRotatable = true;

    // camera should always move after the player moves
    // LateUpdate is called after update
    private void LateUpdate() 
    {
        // Move The camera with the mouse
        if (isRotatable)
        {
            heading += Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;
            tilt += Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
        }
        
        // Only allow values between -80 and 80 for the tilt
        tilt = Mathf.Clamp(tilt, minTilt, maxTilt);

        // Set the new camera rotation
        transform.rotation = Quaternion.Euler(tilt, heading, 0);

        // set the camera position
        transform.position = player.position - transform.forward * camDist + Vector3.up * playerHeight;
    }
}
