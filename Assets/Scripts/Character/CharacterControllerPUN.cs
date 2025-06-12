using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Unity.VisualScripting;
using Photon.Pun;

public class CharacterControllerPun : MonoBehaviourPun, IPunObservable
{
    private Camera cam;               // カメラ（Inspectorからアサイン）
    private NavMeshAgent agent;
    private CancellationTokenSource moveCts;
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    public GameObject canvasObj;
    private CanvasController canvasController;



    //ステータス
    public float moveSpeed = 3.5f;
    public float hp = 1000f;
    public float ap = 100f;

    void Start()
    {
        canvasController = canvasObj.GetComponent<CanvasController>();
        agent = GetComponent<NavMeshAgent>();
        networkPosition = transform.position;
        networkRotation = transform.rotation;


        if (photonView.IsMine)
        {
            cam = GameObject.Find("Main Camera").GetComponent<Camera>();

        }
        canvasController.setIsMyTeam(photonView.IsMine);
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
            transform.position = networkPosition;
            transform.rotation = networkRotation;
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
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);  // ← 向きも送る
        }
        else
        {
            if (stream.IsReading)
            {
                networkPosition = (Vector3)stream.ReceiveNext();
                networkRotation = (Quaternion)stream.ReceiveNext();  // ← 向きも受け取る
            }

        }
    }
}
