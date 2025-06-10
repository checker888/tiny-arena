using UnityEngine;
using UnityEngine.AI;

public class CharacterController : MonoBehaviour
{
    public Camera cam;               // カメラ（Inspectorからアサイン）
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // 右クリック
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }
    }
}
