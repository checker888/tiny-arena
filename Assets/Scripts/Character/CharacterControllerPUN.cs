using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Unity.VisualScripting;
using Photon.Pun;
using UnityEngine.UIElements;

public class CharacterControllerPun : MonoBehaviourPun, IPunObservable
{
    private Camera cam;               // カメラ（Inspectorからアサイン）
    private NavMeshAgent agent;
    private CancellationTokenSource moveCts;
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    public GameObject canvasObj;
    private CanvasController canvasController;
    public GameObject fireballPrefab;
    public int team = 0;

    //ステータス
    public float moveSpeed = 3.5f;
    public int hp = 1000;
    public int ap = 100;

    //ラグ補正
    private Vector3 lastReceivedPos;
    private Vector3 lastReceivedVel;
    private float lastRecvTime;
    private const float interpTime = 0.1f;   // 100 ms で目的地に追従

    void Start()
    {
        canvasController = canvasObj.GetComponent<CanvasController>();
        agent = GetComponent<NavMeshAgent>();
        lastReceivedPos = transform.position;


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
            Vector3 extrapolatedPos = lastReceivedPos + lastReceivedVel * lag;

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

    public void shootFireBall()
    {
        // 1. 生成
        GameObject obj = Instantiate(fireballPrefab, transform.position, transform.rotation);

        // 2. スクリプトを取得して初期化メソッドを呼ぶ
        FireBallController fireball = obj.GetComponent<FireBallController>();
        int power = ap;
        fireball.Initialize(power, team);
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
            lastReceivedPos = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            lastReceivedVel = (Vector3)stream.ReceiveNext();

            lastRecvTime = (float)info.SentServerTime; //送信側時間を記録
        }
    }


}
