using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Cysharp.Threading.Tasks;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField inputField;
    public GameObject RoomNameUI;
    public GameObject ConnectingUI;
    public GameObject LobbyUI;
    public GameObject ChooseModeUI;
    public GameObject button;
    public TMP_Text playerCountText;
    private bool pendingOfflineStart = false;
    void Start()
    {
        ChooseModeUI.SetActive(true);
        LobbyUI.SetActive(false);
        ConnectingUI.SetActive(false);
        RoomNameUI.SetActive(false);

    }

    public void SetUpConnection()
    {
        ChooseModeUI.SetActive(false);
        ConnectingUI.SetActive(true);
        PhotonNetwork.AutomaticallySyncScene = true;
        ConnectToPhoton();
    }


    void ConnectToPhoton()
    {
        Debug.Log("Connecting to Photon...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");

        //オフラインモードならLobbyへ入らない
        if (PhotonNetwork.OfflineMode) return;

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        ConnectingUI.SetActive(false);
        RoomNameUI.SetActive(true);
    }

    public void EnterRoom()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            RoomNameUI.SetActive(false);
            ConnectingUI.SetActive(true);
            EnterToRoom(inputField.text);
        }
    }

    void EnterToRoom(string name)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom(name, new RoomOptions { MaxPlayers = 2 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {

        AssignTeam();
        Debug.Log($"Joined Room. Current player count: {PhotonNetwork.CurrentRoom.PlayerCount}");
        if (PhotonNetwork.OfflineMode)
        {
            // オフラインなら即ゲーム開始
            PhotonNetwork.LoadLevel("NetPlainScene");
            return;
        }
        LobbyUI.SetActive(true);
        RoomNameUI.SetActive(false);
        ConnectingUI.SetActive(false);
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


    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Debug.Log("Master client is starting the game...");
            PhotonNetwork.LoadLevel("NetPlainScene");
        }
    }


    public void SoloPlay()
    {
        if (PhotonNetwork.IsConnected || PhotonNetwork.IsConnectedAndReady)
        {
            pendingOfflineStart = true;
            PhotonNetwork.Disconnect();
        }
        else
        {
            StartOfflineModeAsync().Forget();
        }
    }
    void AssignTeam()
    {
        int team = PhotonNetwork.CurrentRoom.PlayerCount % 2; // 交互に割り当て
        ExitGames.Client.Photon.Hashtable teamProp = new ExitGames.Client.Photon.Hashtable();
        teamProp["team"] = team;
        PhotonNetwork.LocalPlayer.SetCustomProperties(teamProp);

        Debug.Log($"Assigned to team {team}");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (pendingOfflineStart)
        {
            pendingOfflineStart = false;
            StartOfflineModeAsync().Forget();
        }
    }

    private async UniTaskVoid StartOfflineModeAsync()
    {
        PhotonNetwork.OfflineMode = true;
        // Photon の内部状態が ready になるのを待つ
        await UniTask.WaitUntil(() => PhotonNetwork.IsConnectedAndReady);

        Debug.Log("Photon オフライン準備完了 → ルーム参加");
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 1 };
        PhotonNetwork.JoinOrCreateRoom("OfflineRoom", roomOptions, TypedLobby.Default);
    }

}
