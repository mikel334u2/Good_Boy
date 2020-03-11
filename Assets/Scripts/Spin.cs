using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spin : MonoBehaviour
{
	
	[SerializeField] float x = 0;
	[SerializeField] float y = 0;
	[SerializeField] float z = 0;
	
	
	
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
