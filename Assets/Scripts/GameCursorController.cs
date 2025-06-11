using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCursorController : CursorController
{
    public GameObject mainCameraObj;
    private Camera mainCamera;

    private float edgeThreshold = 10f; // 画面端から何ピクセル以内で反応するか
    public float moveSpeed = 10f;     // カメラの移動速度
    private Vector3 initCameraPosition;

    public float toplimit = 25f;
    public float bottomlimit = -25f;
    public float leftlimit = -25f;
    public float rightlimit = 0f;

    void Start()
    {
        mainCamera = mainCameraObj.GetComponent<Camera>();
        isInGame =true;
    }

    void Update()
    {

        UpdateCursorLock();
        DrawMyCursor();
        CameraMove();
    }



    void CameraMove()
    {
        if (!cursorLock) return;

        Vector3 moveDirection = Vector3.zero;
        Vector3 mousePos = Input.mousePosition;

        if (mousePos.x <= edgeThreshold && mainCamera.transform.position.x + Vector3.left.x >= leftlimit)
        {
            moveDirection += Vector3.left;
        }
        if (mousePos.x >= Screen.width - edgeThreshold && mainCamera.transform.position.x + Vector3.right.x <= rightlimit)
        {
            moveDirection += Vector3.right;
        }
        if (mousePos.y >= Screen.height - edgeThreshold && mainCamera.transform.position.z + Vector3.forward.z <= toplimit)
        {
            moveDirection += Vector3.forward;
        }
        if (mousePos.y <= edgeThreshold && mainCamera.transform.position.z >= bottomlimit)
        {
            moveDirection += Vector3.back;
        }

        // ここで mainCamera.transform.position を動かす
        if (moveDirection != Vector3.zero)
        {
            mainCamera.transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }

    }
}
