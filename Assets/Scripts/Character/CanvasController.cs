using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 worldOffset = new Vector3(0, 2.2f, 0); // キャラの頭上
    public GameObject hpBarObj;  
    private Transform target;

    private GameObject mainCameraObj;
    private Camera mainCamera;
    private CharacterControllerPun characterController;
    void Start()
    {
        target = transform.parent;
        mainCameraObj = GameObject.Find("Main Camera");
        mainCamera = mainCameraObj.GetComponent<Camera>();

        if (target != null)
        {
            characterController = target.GetComponent<CharacterControllerPun>();
            if (characterController == null)
            {
                Debug.LogWarning("親に CharacterControllerPun が見つかりませんでした");
            }
        }
        else
        {
            Debug.LogWarning("Canvas の親オブジェクトが見つかりません");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera == null || target == null || hpBarObj == null) return;

        Vector3 worldPos = target.position + worldOffset;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

        if (screenPos.z < 0)
        {
            hpBarObj.SetActive(false);
        }
        else
        {
            hpBarObj.SetActive(true);

            // HPバーの位置をスクリーン座標にセット
            RectTransform rt = hpBarObj.GetComponent<RectTransform>();
            rt.position = screenPos;

            // 🟨 スケール補正（Z距離に応じて反比例で調整）
            float scaleFactor = 1f / screenPos.z;          // 奥に行くほど小さく
            float baseScale = 10f;                          // 基準スケール（調整可）
            rt.localScale = Vector3.one * scaleFactor * baseScale;

            // もし極端に小さくなるのを防ぎたいならClampしてもOK
            //rt.localScale = Vector3.one * Mathf.Clamp(scaleFactor * baseScale, 0.8f, 1.2f);
        }
    }

}
