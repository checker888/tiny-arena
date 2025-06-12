using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public Vector3 worldOffset = new Vector3(0, 2.2f, 0); // キャラの頭上
    public GameObject hpBarObj;
    private Slider hpSlider;

    public GameObject hpBarColorObj;
    private Image hpBarImage;
    private Transform target;

    private GameObject mainCameraObj;
    private Camera mainCamera;
    private int myTeam;
    private CharacterControllerPun characterController;
    void Start()
    {
        target = transform.parent;
        mainCameraObj = GameObject.Find("Main Camera");
        mainCamera = mainCameraObj.GetComponent<Camera>();
        hpBarImage = hpBarColorObj.GetComponent<Image>();
        hpSlider = hpBarObj.GetComponent<Slider>();
        myTeam = (int)PhotonNetwork.LocalPlayer.CustomProperties["team"];



        if(PhotonNetwork.OfflineMode)
        {
            characterController = target.GetComponent<CharacterControllerPun>();
            int targetTeam = characterController.team;
            setUIColor(myTeam, targetTeam);
            return;
        }





        if (target != null)
        {
            characterController = target.GetComponent<CharacterControllerPun>();
            if (characterController != null)
            {
                // このCanvasがついているキャラのチーム番号を取得
                if (characterController.photonView.Owner.CustomProperties.TryGetValue("team", out object t))
                {
                    int targetTeam = (int)t;

                    // ローカルプレイヤーのチームと比較してUI色変更
                    if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("team", out object myTeamObj))
                    {
                        int myTeam = (int)myTeamObj;
                        setUIColor(myTeam, targetTeam);
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("Canvas の親オブジェクトが見つかりません");
        }

    }

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

        }
    }

    public void setUIColor(int myTeam, int targetTeam)
    {
        if (hpBarImage == null)
        {
            hpBarImage = hpBarColorObj.GetComponent<Image>();
        }

        if (myTeam == targetTeam)
        {
            hpBarImage.color = Color.green;
        }
        else
        {
            hpBarImage.color = Color.red;
        }
    }

    public void DamagedBar()
    {
        float maxHP = characterController.maxHP;
        float hp = characterController.hp;
        hpSlider.value = hp / maxHP;


    }
}
