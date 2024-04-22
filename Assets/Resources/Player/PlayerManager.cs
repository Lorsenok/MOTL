using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{

    private PhotonView photonView;
    [SerializeField] public GameObject menu;
    private PlayerController player;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if ( photonView.IsMine )
        {
            player = CreatePlayerController().GetComponent<PlayerController>();
        }
    }

    private GameObject CreatePlayerController()
    {
        Debug.Log("Created player controller");
        return PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), Vector3.zero, Quaternion.identity);
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            if (GetComponentInChildren<Canvas>())
            {
                Destroy(GetComponentInChildren<Canvas>().gameObject);
            }
            return;
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ChangeCanvasState();
        }
        player.GetComponent<PlayerController>().SetCursorVisible(menu.activeSelf);
        player.GetComponent<PlayerController>().canMove = !menu.activeSelf;
    }

    public void ChangeCanvasState()
    {
        menu.SetActive(!menu.activeSelf);
    }

}
