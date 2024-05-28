using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviourPunCallbacks
{

    [SerializeField] public GameObject menu;
    [HideInInspector] public PlayerController player;

    [HideInInspector] public float HP;
    [HideInInspector] public float Speed;
    [HideInInspector] public float DamageMultiplier;
    [HideInInspector] public float Gravity;
    [HideInInspector] public float PlayTime;
    [HideInInspector] public float SpawnTime;
    [HideInInspector] public float SpawnTimeCur = 0;

    [HideInInspector] public bool IsMaster;
    [HideInInspector] public bool IsLeaving = false;

    public int Kills = 0;
    public int Deaths = 0;
    public int Ping = 0;

    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            OnJoinedRoom();
        }
    }

    public void StartGame()
    {
        player = CreatePlayerController().GetComponent<PlayerController>();
    }

    public override void OnJoinedRoom()
    {
        if (!photonView.IsMine) return;

        IsMaster = false;
        photonView.RPC("IsMasterSet", RpcTarget.AllBuffered, PhotonNetwork.IsMasterClient);

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RoomDataSet", RpcTarget.AllBuffered, RoomData.HP, RoomData.Speed, RoomData.DamageMultiplier, RoomData.Gravity, RoomData.PlayTime, RoomData.SpawnTime);
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        if ( photonView.IsMine )
        {
            player = CreatePlayerController().GetComponent<PlayerController>();
        }
    }

    private GameObject CreatePlayerController()
    {
        GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");
        Vector3 spawn = spawners[Random.Range(0, spawners.Length-1)].transform.position;
        GameObject player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawn, Quaternion.identity);
        player.GetComponent<PlayerController>().playerManager = this;
        player.GetComponent<WeaponController>().playerManager = this;
        Debug.Log("Created player controller");
        FindObjectOfType<Settings>().Setup();
        return player;
    }

    private bool once = false;
    private void Update()
    {
        if (!PhotonNetwork.InRoom | IsLeaving) return;
        if (!photonView.IsMine)
        {
            if (GetComponentInChildren<Canvas>())
            {
                Destroy(GetComponentInChildren<Canvas>().gameObject);
            }
            return;
        }

        if (player == null)
        {
            if (SpawnTimeCur > 0)
            {
                SpawnTimeCur -= Time.deltaTime;
            }
            Launcher.SetCursorVisible(true);
            return;
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            foreach (PlayerManager pm in FindObjectsOfType<PlayerManager>())
            {
                if (pm.IsMaster)
                {
                    RoomData.HP = pm.HP;
                    if (!once)
                    {
                        player.HP = pm.HP;
                        once = true;
                    }
                    RoomData.Speed = pm.Speed;
                    player.Speed = pm.Speed;
                    RoomData.DamageMultiplier = pm.DamageMultiplier;
                    RoomData.Gravity = pm.Gravity;
                    player.Gravity = pm.Gravity;
                    RoomData.PlayTime = pm.PlayTime;
                    RoomData.SpawnTime = pm.SpawnTime;
                }
            }

            photonView.RPC("RoomDataSet", RpcTarget.AllBuffered, RoomData.HP, RoomData.Speed, RoomData.DamageMultiplier, RoomData.Gravity, RoomData.PlayTime, RoomData.SpawnTime);
        }

        photonView.RPC("KDPSetup", RpcTarget.AllBuffered, Kills, Deaths, PhotonNetwork.GetPing());

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ChangeCanvasState();
        }
        Launcher.SetCursorVisible(menu.activeSelf);
        player.canMove = !menu.activeSelf;

        if (Input.GetKeyDown(KeyCode.F11))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }

    public void ChangeCanvasState()
    {
        menu.SetActive(!menu.activeSelf);
    }


    [PunRPC] public void RoomDataSet(float hp, float speed, float damageMultiplier, float gravity, float playTime, float spawnTime)
    {
        HP = hp;
        Speed = speed;
        DamageMultiplier = damageMultiplier;
        Gravity = gravity;
        PlayTime = playTime;
        SpawnTime = spawnTime;
    }

    [PunRPC] public void IsMasterSet(bool _isMaster)
    {
        IsMaster = _isMaster;
    }

    [PunRPC] public void KDPSetup(int kills, int deaths, int ping)
    {
        Kills = kills;
        Deaths = deaths;
        Ping = ping;
    }

}
