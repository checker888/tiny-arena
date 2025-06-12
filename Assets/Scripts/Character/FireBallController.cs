using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FireBallController : AttackBase
{
    public GameObject hitEffect;
    void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
        
    }

    void OnTriggerEnter(Collider other)
    {
        Instantiate(hitEffect, transform.position, Quaternion.identity);




        Destroy(gameObject);
    }


}
