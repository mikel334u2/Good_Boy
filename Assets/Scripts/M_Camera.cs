using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Camera : MonoBehaviour
{
    public Transform player;
    public float playerHeight = 1;
    public float heading = 0;
    public float tilt = 15;
    public float minTilt = -80;
    public float maxTilt = 80;
    public float camDist = 10;
    public bool isRotatable = true;
    public bool isRotatableX = true;
    public bool isRotatableY = true;
    public float SensitivityX = 1f;
    public float SensitivityY = 1f;
    private M_PlayerController controller;

    private Controls controls;
    private Vector2 input;
    private void Awake()
    {
        controls = new Controls();
        controls.Player.Look.performed += ctx => input = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => input = Vector2.zero;
    }
    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }

    void Start()
    {
        if (!GameObject.FindGameObjectWithTag("Player").TryGetComponent<M_PlayerController>(out controller))
        {
            Debug.LogWarning("Player controller not found");
        }
    }

    // camera should always move after the player moves
    // LateUpdate is called after update
    private void LateUpdate() 
    {
        // SetAnglesFromMovement();
        HandleInput();

        // Only allow values between -80 and 80 for the tilt
        tilt = Mathf.Clamp(tilt, minTilt, maxTilt);

        // Set the new camera rotation
        transform.rotation = Quaternion.Euler(tilt, heading, 0);

        // set the camera position
        transform.position = player.position - transform.forward * camDist + Vector3.up * playerHeight;
    }

    // Move The camera with the mouse
    private void HandleInput()
    {
        if (!isRotatable) return;
        if (isRotatableX)
        {
            heading += input.x * Time.deltaTime * 180f * SensitivityX;
           // heading += Input.GetAxis("RHorizontal") * Time.deltaTime * 180 * SensitivityX;
        }
        if (isRotatableY)
        {
            tilt += input.y * Time.deltaTime * 180f * SensitivityY;
            //tilt += Input.GetAxis("RVertical") * Time.deltaTime * 180 *SensitivityY;
        }
    }

    private void SetAnglesFromMovement()
    {
        if (controller != null)
        {
            tilt = controller.camAngles.x;
            heading = controller.camAngles.y;
        }
    }
}
