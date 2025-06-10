using UnityEngine;
using UnityEngine.AI;

public class CharacterController : MonoBehaviour
{
    public Camera cam;               // �J�����iInspector����A�T�C���j
    private NavMeshAgent agent;

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
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }
    }

    void KeyPressed()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            agent.SetDestination(this.transform.position);
        }
    }
}
