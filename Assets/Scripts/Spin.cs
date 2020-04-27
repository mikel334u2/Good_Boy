using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spin : MonoBehaviour
{
	
	[SerializeField] float x = 0;
	[SerializeField] float y = 0;
	[SerializeField] float z = 0;
    [SerializeField] bool isWorldSpace = false;
    private Vector3 rotation;

    void Start()
    {
        rotation = new Vector3(x,y,z);
    }

    // Update is called once per frame
    void Update()
    {
    	gameObject.transform.Rotate(rotation * Time.deltaTime, isWorldSpace ? Space.World : Space.Self);
    }
}
