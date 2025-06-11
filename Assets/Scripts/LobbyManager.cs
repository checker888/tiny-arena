using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField inputField;
    public GameObject UI0;
    public GameObject UI1;
    public GameObject UI2;
    public GameObject button;
    public TMP_Text playerCountText;
    void Start()
    {
        UI2.SetActive(false);
        UI1.SetActive(true);
        UI0.SetActive(false);
        PhotonNetwork.AutomaticallySyncScene = true;
        ConnectToPhoton();
    }

    public void EnterRoom()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            UI0.SetActive(false);
            UI1.SetActive(true);
            EnterToRoom(inputField.text);
        }
    }

    void ConnectToPhoton()
    {
        Debug.Log("Connecting to Photon...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        UI1.SetActive(false);
        UI0.SetActive(true);
    }

    void EnterToRoom(string name)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom(name, new RoomOptions { MaxPlayers = 2 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined Room. Current player count: {PhotonNetwork.CurrentRoom.PlayerCount}");
        UI2.SetActive(true);
        UI0.SetActive(false);
        UI1.SetActive(false);
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (playerCount == 2)
        {
            button.SetActive(false);
        }
        playerCountText.text = playerCount.ToString() + "/2";
    }
     

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player entered room. Current player count: {PhotonNetwork.CurrentRoom.PlayerCount}");
        playerCountText.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString()+"/2";
    }


    public void StartButton()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            StartGame();
        }
    }

    void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Master client is starting the game...");
            PhotonNetwork.LoadLevel("NetPlainScene");
        }
    }
}
