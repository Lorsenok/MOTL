using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public enum GameRuleType
{
    Deathmatch
}

public class GameRules : MonoBehaviourPunCallbacks
{
    
    [SerializeField] private TMP_Text wonText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private float timeBeforeRestartSet;
    private float timeBeforeRestart;

    public GameRuleType RuleType;
    public float PlayTime;
    
    private PhotonView pv;

    private bool isEnd = false;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC("Setup", RpcTarget.AllBuffered, RoomData.GameMode, RoomData.PlayTime);
        }
        timeBeforeRestart = timeBeforeRestartSet;
    }

    [PunRPC] public void Setup(GameRuleType type, float time)
    {
        RuleType = type;
        PlayTime = time;
    }

    private bool timeFlowEnded()
    {
        if (PlayTime <= 0)
        {
            return true;
        }
        PlayTime -= Time.deltaTime;
        return false;
    }

    private Player bestPlayer()
    {
        PlayerManager player = FindObjectsOfType<PlayerManager>()[0];
        foreach (PlayerManager pm in FindObjectsOfType<PlayerManager>())
        {
            if (player.Kills < pm.Kills) player = pm;
        }
        return player.GetComponent<PhotonView>().Owner;
    }

    private void win(string text)
    {
        wonText.text = text;
        isEnd = true;
    }

    private bool isLoadingScene = false;
    private void Update()
    {
        switch (RuleType)
        {
            case GameRuleType.Deathmatch:
                if (timeFlowEnded()) win(bestPlayer().NickName + " won!");
                break;
        }

        timeText.text = Mathf.Round(PlayTime).ToString();
        if (PlayTime < 6) timeText.color = Color.red;

        if (isEnd & !isLoadingScene)
        {
            timeBeforeRestart -= Time.deltaTime / Time.timeScale;
            Time.timeScale = 1 / timeBeforeRestartSet * timeBeforeRestart;
            if (timeBeforeRestart <= 0.1f & PhotonNetwork.IsMasterClient)
            {
                timeBeforeRestart = 0.1f;
                PhotonNetwork.LoadLevel("Menu");
                isLoadingScene = true;
                Time.timeScale = 1.0f;
            }
        }
    }
}
