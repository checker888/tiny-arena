using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Unity.VisualScripting;
using Photon.Pun;
using UnityEngine.UIElements;

public class CharacterControllerPun : MonoBehaviourPun, IPunObservable,IDamageable
{
    private Camera cam;               // カメラ（Inspectorからアサイン）
    private NavMeshAgent agent;
    private CancellationTokenSource moveCts;
    public GameObject canvasObj;
    private CanvasController canvasController;
    public GameObject fireballPrefab;
    public int team;

    //ステータス
    public float moveSpeed = 3.5f;
    public int maxHP = 1000;
    public int hp = 1000;
    public int ap = 100;

    //ラグ補正
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private Vector3 lastReceivedVel;
    private float lastRecvTime;
    private const float interpTime = 0.1f;   // 100 ms で目的地に追従

    void Start()
    {
        canvasController = canvasObj.GetComponent<CanvasController>();
        agent = GetComponent<NavMeshAgent>();
        networkPosition = transform.position;


        if (photonView.IsMine)
        {
            cam = GameObject.Find("Main Camera").GetComponent<Camera>();

        }


        if (PhotonNetwork.OfflineMode)
        {
            return;
        }
            team = (photonView.Owner.CustomProperties["team"] is int value) ? value : team;
    }
    

    void Update()
    {
        if (photonView.IsMine)
        {
            MousePressed();
            KeyPressed();
        }
        else
        {
            //ラグ時間ぶん先読み → 線形補間
            float lag = (float)(PhotonNetwork.Time - lastRecvTime);      // 送信から経過した秒数
            Vector3 extrapolatedPos = networkPosition + lastReceivedVel * lag;

            // 補間
            transform.position = Vector3.Lerp(transform.position, extrapolatedPos, Time.deltaTime / interpTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation, Time.deltaTime / interpTime);
        }
        agent.speed = moveSpeed;
        
    }



    void MousePressed()
    {
        if (Input.GetMouseButtonDown(1)) // 右クリック
        {
            StartMoveLoop().Forget();
        }
    }

    void KeyPressed()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StopMoving();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            shootFireBall();
        }
    }

    public void Damaged(int damage)
    {
        hp -= damage;
        canvasController.DamagedBar();
        if(hp < 0)
        {
            Death();
        }
    }

    public void Death()
    {
        Destroy(gameObject);
    }




    public void shootFireBall()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            // ターゲット方向
            Vector3 direction = (hit.point - transform.position).normalized;
            direction.y = 0; // 水平方向のみに限定（空を向かないように）
            
            // 向きたい方向に回転を作成
            Quaternion fireRotation = Quaternion.LookRotation(direction);
            transform.rotation = fireRotation;

            Vector3 spawnPosition = transform.position + Vector3.up * 1.0f; ;

            int damage = ap;
            // 1. 生成
            object[] instData = new object[] { damage, team, transform.forward, spawnPosition };
            GameObject obj = PhotonNetwork.Instantiate("CFXR4 Sun", spawnPosition, fireRotation, 0, instData);
        }

    }

    




    async UniTaskVoid StartMoveLoop()
    {
        // 古いタスクがあればキャンセル
        if (moveCts != null && !moveCts.IsCancellationRequested)
        {
            moveCts.Cancel();
            moveCts.Dispose();
            moveCts = null;
        }

        moveCts = new CancellationTokenSource();
        var token = moveCts.Token;

        try
        {
            while (Input.GetMouseButton(1))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (agent.isOnNavMesh)
                        agent.SetDestination(hit.point);
                }

                await UniTask.Delay(500, cancellationToken: token); // 0.5秒ごとに再設定
            }
        }
        catch (OperationCanceledException)
        {
            // キャンセルされた場合は無視
        }
    }

    void StopMoving()
    {
        if (moveCts != null && !moveCts.IsCancellationRequested)
        {
            moveCts.Cancel();
            moveCts.Dispose();
            moveCts = null;
        }

        // その場で停止（目標位置を現在位置に）
        if (agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(transform.position);
        }
    }















    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)                 // 自分 → ネットへ
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(GetComponent<Rigidbody>()?.velocity ?? Vector3.zero);
        }
        else                                  // ネット → 自分（他者）
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            lastReceivedVel = (Vector3)stream.ReceiveNext();

            lastRecvTime = (float)info.SentServerTime; //送信側時間を記録
        }
    }


}
