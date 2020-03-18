using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGameObject : MonoBehaviour
{
    
    public GameObject BookUI;
    
    
     public void ActivateBook()
    {
        if(BookUI != null)
        {
            bool isActive = BookUI.activeSelf;
            
            BookUI.SetActive(!isActive);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        BookUI.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
