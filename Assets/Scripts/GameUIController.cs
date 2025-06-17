using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    public GameObject Q_UIObj;
    public GameObject W_UIObj;
    public GameObject E_UIObj;
    public GameObject R_UIObj;

    private Image Q_UIImage;
    private Image W_UIImage;
    private Image E_UIImage;
    private Image R_UIImage;

    private TextMeshProUGUI Q_UIText;
    private TextMeshProUGUI W_UIText;
    private TextMeshProUGUI E_UIText;
    private TextMeshProUGUI R_UIText;

    private CancellationTokenSource qCts;

    // Start is called before the first frame update
    void Start()
    {
        Q_UIImage = Q_UIObj.GetComponent<Image>();
        W_UIImage = W_UIObj.GetComponent <Image>();
        E_UIImage = E_UIObj.GetComponent< Image>();
        R_UIImage = R_UIObj.GetComponent<Image>() ;

        Q_UIText = Q_UIObj.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        W_UIText = W_UIObj.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        E_UIText = E_UIObj.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        R_UIText = R_UIObj.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

    }



    public async UniTaskVoid ActivateQ(float cooltime)
    {
        qCts?.Cancel();
        qCts = new CancellationTokenSource();
        var token = qCts.Token;

        float timeLeft = cooltime;
        Q_UIImage.color = Color.gray;

        try
        {
            while (timeLeft > 0)
            {
                Q_UIText.text = timeLeft.ToString("F1");
                await UniTask.Delay(100, cancellationToken: token);
                timeLeft -= 0.1f;
            }
        }
        catch (OperationCanceledException)
        {
            return; // キャンセルされたらここで抜ける
        }

        Q_UIImage.color = Color.white;
        Q_UIText.text = "Q";
    }

    public async UniTaskVoid ActivateW(float cooltime)
    {

    }
    public async UniTaskVoid ActivateE(float cooltime)
    {

    }
    public async UniTaskVoid ActivateR(float cooltime)
    {

    }
}
