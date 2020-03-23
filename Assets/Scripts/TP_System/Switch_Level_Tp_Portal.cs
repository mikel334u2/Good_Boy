using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Switch_Level_Tp_Portal : MonoBehaviour
{

    public string levelToLoad01;
    

    void OnTriggerEnter(Collider myCollider)
    {
        if (myCollider.gameObject.name == "Player")
        {
            SceneManager.LoadScene(levelToLoad01);
        }
        
    }
}
