using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Unity.VisualScripting;

public class CharacterController : MonoBehaviour
{


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
