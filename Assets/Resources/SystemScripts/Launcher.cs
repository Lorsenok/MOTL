using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{

    public static Launcher Instance { get; private set; }

    [SerializeField] private GameObject roomManagerPrefab;
    [SerializeField] private GameObject logText;
    [SerializeField] private TMPro.TMP_InputField createRoomInputField;
    [SerializeField] private TMPro.TMP_Text roomName;
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomListPrefab;
    [SerializeField] private Transform playerListContent;
    [SerializeField] private GameObject playerListPrefab;
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private GameObject setupGameButton;
    [SerializeField] private TMPro.TMP_InputField changeNicknameInputField;
    [SerializeField] private GameObject roomMenu;

    private static bool isNickameChanged = false;
    private static string nickname;

    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Debug.Log("Connecting a server");
        SetCursorVisible(true);
        if (!PhotonNetwork.InRoom)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.Disconnect();
            PhotonNetwork.ConnectUsingSettings();
            DontDestroyOnLoad(Instantiate(roomManagerPrefab));
        }
        else
        {
            GetComponent<MenuManager>().MenuOpen("room");
            PhotonNetwork.CurrentRoom.IsOpen = true;
            UpdatePlayerList();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
        if (roomMenu.activeSelf)
        {
            UpdatePlayerList();
        }
    }

    public static void SetCursorVisible(bool visible)
    {
        Cursor.visible = visible;
        if (visible) Cursor.lockState = CursorLockMode.None;
        if (!visible) Cursor.lockState = CursorLockMode.Locked;
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
        if (nickname != null)
        {
            isNickameChanged = true;
            changeNicknameInputField.text = nickname;
            PhotonNetwork.NickName = changeNicknameInputField.text;
        }
    }

    public void OnNicknameChanged()
    {
        if (changeNicknameInputField.text.Length > 5)
        {
            isNickameChanged = true;
            PhotonNetwork.NickName = changeNicknameInputField.text;
            nickname = PhotonNetwork.NickName;
        }
        else
        {
            logText.GetComponent<LogText>().Message("Nickname must have more than 5 symbols");
        }
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
        if (!isNickameChanged)
        {
            PhotonNetwork.NickName = "Player" + Random.Range(-1000, 1000);
        }

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
            Debug.Log("removed " + t.name);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList) continue;
            Instantiate(roomListPrefab, roomListContent).GetComponent<RoomButton>().SetUp(roomList[i]);
            Debug.Log("added " + roomList[i].Name);
        }
    }

    private void UpdatePlayerList()
    {
        foreach (Transform t in playerListContent)
        {
            Destroy(t.gameObject);
        }
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Instantiate(playerListPrefab, playerListContent).GetComponent<PlayerText>().SetUp(player);
        }
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
