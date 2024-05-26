using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LauncherInGameMenu : MonoBehaviourPunCallbacks
{
    private PhotonView pv;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject eventSystem;

    private bool isLeaving = false;

    private GameInterface gameInterface;
    private PlayerManager pm;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        if (!pv.IsMine)
        {
            Destroy(canvas);
            Destroy(eventSystem);
            return;
        }
        gameInterface = GetComponentInChildren<GameInterface>();
        pm = GetComponent<PlayerManager>();
    }

    private void OnDestroy()
    {
        if (pv.IsMine) OnLeftRoom();
    }

    public void LeaveRoom()
    {
        if (!pv.IsMine | isLeaving) return;
        isLeaving = true;
        Debug.Log("Leaving room");
        Destroy(FindObjectOfType<RoomManager>().gameObject);
        foreach (PlayerManager pm in FindObjectsOfType<PlayerManager>())
        {
            pm.IsLeaving = true;
        }
        foreach (PhotonView _pv in FindObjectsOfType<PhotonView>())
        {
            PhotonNetwork.OpCleanRpcBuffer(_pv);
        }
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        if (!pv.IsMine) return;
        Debug.Log("Room left");
        SceneManager.LoadScene("Menu");
    }

    private void Update()
    {
        if (!pv.IsMine) return;
        gameInterface.gameObject.SetActive(pm.player != null);
    }
}
