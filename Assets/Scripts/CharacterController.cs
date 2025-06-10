using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Unity.VisualScripting;

public class CharacterController : MonoBehaviour
{
    public Camera cam;               // �J�����iInspector����A�T�C���j
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
        if (Input.GetMouseButtonDown(1)) // �E�N���b�N
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
        // �Â��^�X�N������΃L�����Z��
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

                await UniTask.Delay(500, cancellationToken: token); // 0.5�b���ƂɍĐݒ�
            }
        }
        catch (OperationCanceledException)
        {
            // �L�����Z�����ꂽ�ꍇ�͖���
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

        // ���̏�Œ�~�i�ڕW�ʒu�����݈ʒu�Ɂj
        if (agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(transform.position);
        }
    }
}
