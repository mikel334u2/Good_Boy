using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch_Level_Tp_Portal : MonoBehaviour
{

    public string levelToLoad01;
    

    void OnTriggerEnter(Collider myCollider)
    {
        if (myCollider.gameObject.name == "Player")
        {
            Application.LoadLevel(levelToLoad01);
        }
        
    }
}
