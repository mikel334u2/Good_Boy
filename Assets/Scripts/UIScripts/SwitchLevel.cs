using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchLevel : MonoBehaviour
{
    public string levelToLoad;

    //Update is called once per frame
    public void LoadTheLevel ()
    {
        SceneManager.LoadScene(levelToLoad);

    }
    private void OnTriggerEnter(Collider other){
    	 SceneManager.LoadScene(levelToLoad);
    }
}
