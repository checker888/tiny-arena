using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public abstract class AttackBase : MonoBehaviourPun, IPunObservable, IPunInstantiateMagicCallback
{ 
    public float speed = 12f;
    public float range = 100f;
    public int damage;
    public int team;
    public Vector3 startPos;
    protected Vector3 velocity;

    //ラグ補正
    protected Vector3 networkPosition;
    protected Quaternion networkRotation;
    protected Vector3 lastReceivedVel;
    protected float lastRecvTime;
    protected const float interpTime = 0.1f;

    protected virtual void Start()
    {
        startPos = transform.position;
        if (photonView.IsMine)
        {
            velocity = transform.forward * speed; // 自前で速度設定
        }

    }
    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
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

    public virtual void Initialize(int damage, int team)
    {
        this.damage = damage;
        this.team = team;
    }
    public virtual void Initialize(int damage, int team, Vector3 forward, Vector3 startPos)
    {
        this.damage = damage;
        this.team = team;
        this.startPos = startPos;
        this.velocity = forward.normalized * speed;
    }

    //ラグ補正して座標更新する関数　子クラスのUpdate関数で必ず呼び出す
    protected virtual void LagCompensation()
    {
        if (photonView.IsMine)
            return;

        // ラグ補正：予測 + 補間
        float lag = (float)(PhotonNetwork.Time - lastRecvTime);
        Vector3 extrapolatedPos = networkPosition + lastReceivedVel * lag;

        transform.position = Vector3.Lerp(transform.position, extrapolatedPos, Time.deltaTime / interpTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation, Time.deltaTime / interpTime);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(velocity);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            lastReceivedVel = (Vector3)stream.ReceiveNext();
            lastRecvTime = (float)info.SentServerTime;
        }
    }
}
