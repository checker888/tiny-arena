using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FireBallController : MonoBehaviourPun, IPunObservable
{
    public GameObject hitEffect;
    public float speed = 12f;
    public Vector3 startPos;
    void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
        startPos = transform.position;
    }

    public void Initialize(int power, int team)
    {

    }

    void OnTriggerEnter(Collider other)
    {
        Instantiate(hitEffect, transform.position, Quaternion.identity);




        Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
