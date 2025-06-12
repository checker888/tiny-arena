using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public abstract class AttackBase : MonoBehaviourPun, IPunObservable
{ 
    public float speed = 12f;
    public float range = 100f;
    public int damage;
    public int team;
    public Vector3 startPos;
    void Start()
    {
        startPos = transform.position;
    }

    public virtual void Initialize(int damage, int team)
    {
        this.damage = damage;
        this.team = team;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
