using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject tempCharaBlue;
    public GameObject tempCharaRed;

    void Start()
    {
        DeleteTempCharacter();


        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("MaleCharacterPolyart", new Vector3(-27.3f, 0.05f, 15.0f), Quaternion.Euler(0, 90, 0));
        }
        else if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("FemaleCharacterPolyart", new Vector3(-2.5f, 0.05f, 15.0f), Quaternion.Euler(0, -90, 0));
        }
    }


    void DeleteTempCharacter()
    {
       if(tempCharaBlue != null)
        {
            tempCharaBlue.SetActive(false);
        }
       if (tempCharaRed != null)
        {
            tempCharaRed.SetActive(false);
        }
    }
}
