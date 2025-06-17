using Cysharp.Threading.Tasks;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class MagicCircleController : AttackBase
{
    private float maxSize = 2f;
    private float nowSize = 0;
    private Collider myCol;
    // Start is called before the first frame update
    protected override void Start()
    {
        myCol = GetComponent<Collider>();
        nowSize = 0;
        transform.localScale = Vector3.zero;
        base.Start();
        ExpandCircle().Forget();
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


    // Update is called once per frame
    void Update()
    {
        
    }
    public async UniTaskVoid ExpandCircle()
    {

       
        try
        {
            while (nowSize <= maxSize)
            {
                transform.localScale = Vector3.one * nowSize;
                await UniTask.Delay(10);
                nowSize+=0.02f;

            }
            await UniTask.Delay(300);
        }
        catch (OperationCanceledException)
        {
            return; // キャンセルされたらここで抜ける
        }


        Collider[] hits = Physics.OverlapBox(myCol.bounds.center, myCol.bounds.extents, transform.rotation);
        foreach (Collider col in hits)
        {
            if (col.CompareTag("Player"))
            {
                CharacterControllerPun character = col.GetComponent<CharacterControllerPun>();
                if (character != null && character.team != this.team) 
                {
                    character.Damaged(damage); 
                }
            }
        }
        PhotonNetwork.Destroy(gameObject);
    }
}
