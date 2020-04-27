using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/**************************************************************************************************
 *                                                                                                *
 * These scripts, in this state are heavily influenced from this tutorial by Nimso Ny:   *
 * https://www.youtube.com/watch?v=zXv7P6avhHM&list=PLoLkfS-a4sd2nPoeTkO-rj_Kj0FkKZYlw&index=3    *
 *                                                                                                *
 **************************************************************************************************
 *
 * I highly reccomend following his tutorial series for programmers, it's a great refresher.
 *
 * Additionaly, sample scripts onward will use this design as a base. :)
 *
 */

// A Simple 3D Player Controller script that uses Unity's PlayerController component
public class M_PlayerController : MonoBehaviour
{
    // Objects
    private CharacterController controller;
    private Transform cam;

    // Camera
    private Vector3 camF, camR;
    [HideInInspector] public Vector3 camAngles;

    // Physics
    Vector3 intent;
    [HideInInspector] public Vector3 velocity = new Vector3(0,0,0);
    public float speed = 10f;
    [HideInInspector] public bool isSprinting = false;
    public float sprintSpeed = 20;
    private float adjustedSpeed;
    
    public float jumpVelocity = 10;
    public float bounceVelocity = 50;
    public float acceleration = 11;
    private float turnSpeed = 5f;
    public float turnSpeedLow = 7f;
    public float turnSpeedHigh = 20f;
    private Vector3 forward;
    private RaycastHit hit;

    // Gravity
    public float grav = 9.81f;
    [HideInInspector] public bool grounded = false;
    private bool canJump = true;
    [HideInInspector] public Animator animator; //TEST used to be private
    public float raycastDistance = .2f;
    private bool doRaycast = true;
    [HideInInspector] public bool zeroMovement = false;

    // bork
    public AudioSource barkSound;

    // New input system
    private Controls controls;
    private Vector2 input = new Vector2(0,0);
    private bool jumpInput = false;
    private bool barkInput = false;
    private bool twerkInput = false;
    private void Awake()
    {
        controls = new Controls();
        controls.Player.Move.performed += ctx => input = ctx.ReadValue<Vector2>();
        controls.Player.Bark.performed += ctx => barkInput = true;
        controls.Player.Jump.performed += ctx => jumpInput = true;
        controls.Player.Sprint.performed += ctx => isSprinting = true;
        controls.Player.Twerk.performed += ctx => twerkInput = true;
        controls.Player.Move.canceled += ctx => input = Vector2.zero;
        controls.Player.Bark.canceled += ctx => barkInput = false;
        controls.Player.Jump.canceled += ctx => jumpInput = false;
        controls.Player.Sprint.canceled += ctx => isSprinting = false;
        controls.Player.Twerk.canceled += ctx => twerkInput = false;
    }
    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
    
    private void Start() 
    {
        // load the CharacterController attatched to this object
        controller = GetComponent<CharacterController>();
        if (!TryGetComponent<Animator>(out animator))
        {
            Debug.Log("Add an Animator to your player");
        }
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        adjustedSpeed = speed;
    }
   
    private void Update()
    {
        if (!zeroMovement)
            DoInput();
        CalculateCamera();
        CalculateGround();
        CalculateForward();
        DoMove();
        DoGravity();
        DoJump();
        //DoAttack();

        HandleMovement();
        // Debug.Log(velocity);
    }

    // Handles various inputs
    // Sets "input" variable
    void DoInput()
    {
        adjustedSpeed = (isSprinting) ? sprintSpeed : speed;

        // animator.SetBool("Sprint", isSprinting); //TEST does not fully work, as Sprint as no deactivtion
        animator.SetBool("Jumping", jumpInput);
        animator.SetBool("Twerking", twerkInput);
        animator.SetBool("Barking", barkInput);
        if (barkInput)
        {
            barkSound.Play();
        }
    }
    
    // sets variables representing the orientation of the camera
    // this way we will move dependent on our camera rotation
    // sets "camF" and "camR" variables
    void CalculateCamera()
    {
        camF = cam.forward;
        camR = cam.right;

        camF.y = 0;
        camF = camF.normalized;

        camR.y = 0;
        camR = camR.normalized;
    }

    // lets us know if our character is grounded by performing a raycast
    // from the player to the ground
    // sets "grounded" and "hit" variables
    void CalculateGround()
    {
        // if raycast allowed and hit detected and hit is not a trigger collider
        grounded = doRaycast
            && Physics.Raycast(transform.position + Vector3.up * 0.1f, -Vector3.up, out hit, raycastDistance)
            && !hit.collider.isTrigger;

        // Debug.Log("Standing on: " + hit.transform.gameObject.name);
        animator.SetBool("Grounded", grounded); // set animator value of grounded
        // Debug.Log(velocity.y);
    }

    // sets "forward" variable relative to grounded position
    void CalculateForward()
    {
        forward = (grounded) ? Vector3.Cross(transform.right, hit.normal) : transform.forward;
    }
    
    // set the velocity for movement with respect to player input and camera orientation
    void DoMove()
    {
        input = Vector2.ClampMagnitude(input, 1);

        // this is the direction that we will move in based on input and camera orientation
        intent = (camF * input.y + camR * input.x);

        // this is the speed the character will rotate (turn) at
        // we want to base the turn speed based off of how fast we are moving
        // i.e. turn quickly if we are still and slower if we are moving quickly
        turnSpeed = Mathf.Lerp(turnSpeedHigh, turnSpeedLow, velocity.magnitude / adjustedSpeed);
        
        // if we are getting movement input
        // turn the character to face the direction of movement
        // also turn the camera if applicable
        if (input.magnitude > 0 && !zeroMovement)
        {
            Quaternion rot = Quaternion.LookRotation(intent);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
            Quaternion camRot = Quaternion.Slerp(cam.transform.rotation, rot, turnSpeed * Time.deltaTime);
            camAngles = camRot.eulerAngles;
        }
        
        // seperate out the y component of velocity so that it does not affect our XZ movement
        Vector3 velocityXZ = velocity;
        velocityXZ.y = 0;

        // get the appropriate velocity (which accounts for direction) and apply speed 
        // and Linearly Interpolate based off of rotation (start moving slower and then speed up)
        // then we can add the y velocity back in
        velocityXZ = Vector3.Lerp(velocityXZ, forward * input.magnitude * adjustedSpeed, acceleration* Time.deltaTime);
        velocity = new Vector3(velocityXZ.x, velocity.y, velocityXZ.z);
    }

    // apply gravity, but only speed up when we are not grounded
    void DoGravity()
    {
        if (grounded)
        {
            // animator.SetTrigger("Land");
            velocity.y = -0.5f;
        }
        else
        {
            velocity.y -= grav * Time.deltaTime;
            if (velocity.y <= 0)
            {
                doRaycast = true;
            }
        }
        velocity.y = Mathf.Clamp(velocity.y, -100, 100);
    }

    // Jump, by using the velocity we have set up
    // but only if the character is grounded
    void DoJump()
    {
        if (!zeroMovement && grounded && jumpInput)
        {
            doRaycast = false;
            velocity.y = jumpVelocity;
            // animator.SetTrigger("Jump");
        }
    }
    /*void DoAttack()
    {
        if (Input.GetButtonDown("Fire1") && grounded)
        {
            animator.SetTrigger("Attack");
        }
        
    }*/
    void HandleMovement()
    {
        // zero velocity if zeroMovement is true
        if (zeroMovement)
        {
            velocity.x = 0;
            velocity.z = 0;
        }
        Vector3 timedVelocity = velocity * Time.deltaTime;
        controller.Move(timedVelocity);
        timedVelocity.y = 0;
        animator.SetFloat("MoveSpeed", timedVelocity.magnitude * 10);
    }

    public void StopMovement()
    {
        animator.SetFloat("MoveSpeed", 0);
        this.enabled = false;
    }
    
    void OnTriggerEnter(Collider collision) 
    {
    	if (collision.tag == "Bounce"){
    		
    		velocity.y = bounceVelocity;
    		doRaycast = false;
    	}
    	 
    }
    
}
