using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprint : StateMachineBehaviour
{
	bool sprint;
	public float speed;
	Animator animator;
	public GameObject Player;
	
	// Start is called before the first frame update
    void Start()
    {
    	//speed = Player.GetComponent<M_PlayerController>().speed = 20;
    	//sprint = Player.GetComponent<M_PlayerController>().sprint();
    	//animator = Player.GetComponent<M_PlayerController>().animator();
    
    	
    	
    	sprint = GameObject.Find("Player").GetComponent<M_PlayerController>().sprint;
    	speed = GameObject.Find("Player").GetComponent<M_PlayerController>().speed = 20;
    	animator = GameObject.Find("Player").GetComponent<M_PlayerController>().animator;
    }

    // Update is called once per frame
    void Update()
    {
       if (Input.GetButtonDown("Sprint") && sprint == true){
        	sprint = false;
        } animator.SetBool("Sprint", sprint); //TEST does not fully work, as Sprint as no deactivtion 
    }
}
