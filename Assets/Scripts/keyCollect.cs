using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class keyCollect : MonoBehaviour
{
    public GameObject key3;
    public GameObject key2;
    public GameObject key1;
    public GameObject sticker3;
    public GameObject sticker2;
    public GameObject sticker1;
	
	
	 void Start()
    {
        key3.SetActive(false);
        key2.SetActive(false);
        key1.SetActive(false);
        sticker3.SetActive(false);
        sticker2.SetActive(false);
        sticker1.SetActive(false);
    }

	
	
	private void OnTriggerEnter(Collider collision)
    {
		if (collision.CompareTag("Key"))
        {
            AddKey();
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("Sticker"))
        {
            AddSticker();
            Destroy(collision.gameObject);
        }
    }
	  
    private void AddKey()
    {
        if(!key1.activeInHierarchy)
        {
            key1.SetActive(true);
        }
        else if (!key2.activeInHierarchy)
        {
            key2.SetActive(true);
        }
        else if (!key3.activeInHierarchy)
        {
            key3.SetActive(true);
        }   
    }

    private void AddSticker()
    {
        if(!key1.activeInHierarchy)
        {
            sticker1.SetActive(true);
        }
        else if (!key2.activeInHierarchy)
        {
            sticker2.SetActive(true);
        }
        else if (!key3.activeInHierarchy)
        {
            sticker3.SetActive(true);
        }
        
    }
}