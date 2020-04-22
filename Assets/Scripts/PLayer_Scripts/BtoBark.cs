using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Maria Bark thingy
public class BtoBark : MonoBehaviour {
    public AudioSource barkSound;

    void Start(){

    }
    void Update(){
        if(Input.GetKey("b")==true){
            barkSound.Play();
        }
    }
}