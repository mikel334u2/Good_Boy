 using UnityEngine;
 using System.Collections;
  using System.Collections.Generic;
 
 public class jumpPlatform : MonoBehaviour
 {
     public float speed = 5f;
     public Rigidbody rigidPlayer;
 
     private Rigidbody rigid;
 
     void Start ()
     {
         rigid = transform.GetComponent<Rigidbody>();
     }
 
     void OnCollisionEnter(Collision collidedWithThis)
     {
         if (collidedWithThis.transform.tag == "Player")
         {
             rigidPlayer.velocity = transform.up * speed;
         }
     }
 }
