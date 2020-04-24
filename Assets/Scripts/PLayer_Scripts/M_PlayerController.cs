using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Input
    
    private Vector2 input;

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
        DoInput();
        CalculateCamera();
        CalculateGround();
        CalculateForward();
        DoMove();
        DoGravity();
        if (canJump)
        {
            DoJump();
        }
        //DoAttack();

        HandleMovement();
        // Debug.Log(velocity);
    }

    // Handles various inputs
    // Sets "input" variable
    void DoInput()
    {
        if (zeroMovement)
            return;
        
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        input = Vector2.ClampMagnitude(input, 1);

        if (Input.GetButtonDown("Sprint"))
        {
        	isSprinting = !isSprinting;
        	adjustedSpeed = (isSprinting) ? sprintSpeed : speed;
        }

        // animator.SetBool("Sprint", isSprinting); //TEST does not fully work, as Sprint as no deactivtion
        animator.SetBool("Jumping", Input.GetButtonDown("Jump"));
        animator.SetBool("Twerking", Input.GetButtonDown("Twerk"));
        animator.SetBool("Barking", Input.GetButtonDown("Bark"));
        if (Input.GetButtonDown("Bark"))
        {
            barkSound.Play();
        }

        if (Input.GetButtonDown("Cancel"))
        {
            Debug.Log("Game exiting");
            Application.Quit();
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
        // this is the direction that we will move in based on input and camera orientation
        intent = (camF * input.y + camR * input.x);

        // this is the speed the character will rotate (turn) at
        // we want to base the turn speed based off of how fast we are moving
        // i.e. turn quickly if we are still and slower if we are moving quickly
        turnSpeed = Mathf.Lerp(turnSpeedHigh, turnSpeedLow, velocity.magnitude / adjustedSpeed);
        
        // if we are getting movement input
        // turn the character to face the direction of movement
        if (input.magnitude > 0 && !zeroMovement)
        {
            Quaternion rot = Quaternion.LookRotation(intent);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
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
        if (grounded && Input.GetButtonDown("Jump"))
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
        zeroVelocityXZ(); // zero velocity if zeroMovement is true
        Vector3 timedVelocity = velocity * Time.deltaTime;
        controller.Move(timedVelocity);
        timedVelocity.y = 0;
        animator.SetFloat("MoveSpeed", timedVelocity.magnitude * 10);
    }

    // public void StopMovement()
    // {
    //     animator.SetFloat("MoveSpeed", 0);
    //     this.enabled = false;
    // }
    void zeroVelocityXZ()
    {
        if(zeroMovement)
        {
            velocity.x = 0;
            velocity.z = 0;
            canJump = false;
           
        }
        else
        {
            canJump = true;
            
        }
    }
    
    void OnTriggerEnter(Collider collision) 
    {
    	if (collision.tag == "Bounce"){
    		
    		velocity.y = bounceVelocity;
            Debug.Log("Bounce " + velocity.y);
    		doRaycast = false;
    	}
    	 
    }
    
}
