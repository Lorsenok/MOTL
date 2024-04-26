using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks
{

    public static Launcher Instance { get; private set; }

    [SerializeField] GameObject logText;
    [SerializeField] TMPro.TMP_InputField createRoomInputField;
    [SerializeField] TMPro.TMP_Text roomName;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListPrefab;
    [SerializeField] GameObject startGameButton;
    [SerializeField] GameObject setupGameButton;

    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Debug.Log("Connecting a server");
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.MenuOpen("title");
        Debug.Log("Joined a lobby");
    }

    public void CreateRoom()
    {
        if (createRoomInputField.text.Length <= 3) 
        {
            logText.GetComponent<LogText>().Message("Your room name should have more than 3 symbols");
        }
        else
        {
            PhotonNetwork.CreateRoom(createRoomInputField.text);
            MenuManager.Instance.MenuOpen("loading");
            Debug.Log("Joining room");
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room created");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.MenuOpen("room");
        Debug.Log("Room joined");
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        PhotonNetwork.NickName = "Player" + Random.Range(-1000, 1000);

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(playerListPrefab, playerListContent).GetComponent<PlayerText>().SetUp(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        setupGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        setupGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        logText.GetComponent<LogText>().Message(message);
        Debug.Log(returnCode);
        MenuManager.Instance.MenuOpen("title");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        Debug.Log("Joining room");
        MenuManager.Instance.MenuOpen("loading");
    }

    public void LeaveRoom()
    {
        Debug.Log("Leaving room");
        MenuManager.Instance.MenuOpen("loading");
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Room left");
        MenuManager.Instance.MenuOpen("title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform t in roomListContent) 
        { 
            Destroy(t.gameObject); 
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList) continue;
            Instantiate(roomListPrefab, roomListContent).GetComponent<RoomButton>().SetUp(roomList[i]); 
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListPrefab, playerListContent).GetComponent<PlayerText>().SetUp(newPlayer);
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel("TestMap");
    }

    public void Exit()
    {
        Application.Quit();
    }

}
