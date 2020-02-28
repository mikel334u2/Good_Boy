using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
	
	[SerializeField] float x;
	[SerializeField] float y;
	[SerializeField] float z;
	
	
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    	gameObject.transform.Rotate(z,x,y);
    }
}
