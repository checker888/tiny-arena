using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Unity.VisualScripting;
using static Photon.Pun.UtilityScripts.PunTeams;

public class CharacterController : MonoBehaviour
{
    public GameObject fireballPrefab;
    public int team = 0;

    //ステータス
    public float moveSpeed = 3.5f;
    public int hp = 1000;
    public int ap = 100;

    public Camera cam;               // カメラ（Inspectorからアサイン）
    private NavMeshAgent agent;
    private CancellationTokenSource moveCts;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();


    }

    void Update()
    {
        MousePressed();
        KeyPressed();
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
        if(Input.GetKeyDown(KeyCode.S))
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
        int damage = ap;
        fireball.Initialize(ap, team, transform.forward, transform.position); // ← forward と startPos 両方明示
    }




    async UniTaskVoid StartMoveLoop()
    {
        // 古いタスクがあればキャンセル
        StopMoving();

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
}
