using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch_Level_Tp : MonoBehaviour
{

    public string levelToLoad;

    void OnTriggerEnter(Collider myCollider)
    {
        if (myCollider.gameObject.name == "Portal_Invisible")
        {
            Application.LoadLevel(levelToLoad);
        }
    }
}
