using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviourPunCallbacks
{

    private PhotonView photonView;
    [SerializeField] public GameObject menu;
    private PlayerController player;

    public int HP;
    public float Speed;
    public float DamageMultiplier;
    public float Gravity;
    public float PlayTime;
    public float SpawnTime;

    public bool isMaster;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            OnJoinedRoom();
        }
    }

    public override void OnJoinedRoom()
    {
        if (!photonView.IsMine) return;

        photonView.RPC("IsMasterSet", RpcTarget.AllBuffered, PhotonNetwork.IsMasterClient);

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RoomDataSet", RpcTarget.AllBuffered, RoomData.HP, RoomData.Speed, RoomData.DamageMultiplier, RoomData.Gravity, RoomData.PlayTime, RoomData.SpawnTime);
        }
        if ( photonView.IsMine )
        {
            player = CreatePlayerController().GetComponent<PlayerController>();
        }
    }

    private GameObject CreatePlayerController()
    {
        Debug.Log("Created player controller");
        GameObject player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), Vector3.zero, Quaternion.identity);
        return player;
    }

    private void Update()
    {
        if (!PhotonNetwork.InRoom | player == null) return;
        if (!photonView.IsMine)
        {
            if (GetComponentInChildren<Canvas>())
            {
                Destroy(GetComponentInChildren<Canvas>().gameObject);
            }
            return;
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            foreach (PlayerManager pm in FindObjectsOfType<PlayerManager>())
            {
                if (pm.isMaster)
                {
                    RoomData.HP = pm.HP;
                    player.HP = pm.HP;
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

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ChangeCanvasState();
        }
        player.SetCursorVisible(menu.activeSelf);
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


    [PunRPC] public void RoomDataSet(int hp, float speed, float damageMultiplier, float gravity, float playTime, float spawnTime)
    {
        HP = hp;
        Speed = speed;
        DamageMultiplier = damageMultiplier;
        Gravity = gravity;
        PlayTime = playTime;
        SpawnTime = spawnTime;
        Gravity = gravity;
        PlayTime = playTime;
        SpawnTime = spawnTime;
    }

    [PunRPC] public void IsMasterSet(bool _isMaster)
    {
        isMaster = _isMaster;
    }

}
