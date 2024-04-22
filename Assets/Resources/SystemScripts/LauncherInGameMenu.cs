﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LauncherInGameMenu : MonoBehaviourPunCallbacks
{

    private PhotonView pv;
    [SerializeField] private GameObject eventSystem;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        if (!pv.IsMine)
        {
            Destroy(eventSystem);
            return;
        }
    }

    private void OnDestroy()
    {
        if (pv.IsMine) OnLeftRoom();
    }

    public void LeaveRoom()
    {
        if (!pv.IsMine) return;
        Debug.Log("Leaving room");
        Destroy(FindObjectOfType<RoomManager>().gameObject);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        if (!pv.IsMine) return;
        Debug.Log("Room left");
        SceneManager.LoadScene("Menu");
    }
}
