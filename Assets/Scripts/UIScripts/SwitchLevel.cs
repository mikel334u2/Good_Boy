using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchLevel : MonoBehaviour
{
    public string levelToLoad;

    //Update is called once per frame
    public void LoadTheLevel ()
    {
        Application.LoadLevel(levelToLoad);

    }
}
