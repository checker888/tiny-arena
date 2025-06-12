using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class FireBallController : AttackBase
{
    //public GameObject hitEffect;
    protected override void Start()
    {
        speed = 10f;
        range = 8.0f;
        base.Start();

    }

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = photonView.InstantiationData;
        if (data != null && data.Length >= 4)
        {
            int damage = (int)data[0];
            int team = (int)data[1];
            Vector3 forward = (Vector3)data[2];
            Vector3 position = (Vector3)data[3];

            Initialize(damage, team, forward, position);
        }
    }


    private void Update()
    {

       

        if (photonView.IsMine)
        {
            transform.position += velocity * Time.deltaTime;
        }

        float distanceMoved = Vector3.Distance(startPos, transform.position);
        if (distanceMoved >= range)
        {
            Destroy(gameObject);
        }





        LagCompensation();

    }
    void OnTriggerEnter(Collider other)
    {

        //Instantiate(hitEffect, transform.position, Quaternion.identity);
        if (other.gameObject.tag != "Player" || team == 0) return;

        CharacterControllerPun characterController = other.gameObject.GetComponent<CharacterControllerPun>();

        if(characterController.team != this.team) 
        {
            Debug.Log(characterController.team+" "+this.team);
            characterController.Damaged(damage);
            Destroy(gameObject);
        }

        
    }


}
