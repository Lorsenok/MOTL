using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class ScoreboardItem : MonoBehaviour
{
    [SerializeField] private TMP_Text nicknameText;
    [SerializeField] private TMP_Text killsText;
    [SerializeField] private TMP_Text deathsText;
    [SerializeField] private TMP_Text pingText;

    public void Initialize(Player player)
    {
        nicknameText.text = player.NickName;
    }

    private void Update()
    {
        foreach (PlayerManager playerManager in FindObjectsOfType<PlayerManager>())
        {
            string pmname = playerManager.gameObject.GetComponent<PhotonView>().Owner.ToString();
            if (playerManager.gameObject.GetComponent<PhotonView>().Owner.ToString() == "#" + pmname[1] + pmname[2] + " '" + nicknameText.text + "'") //Каким то хером Owner возвращает не только никнейм, но еще и
            {                                                                                                                                         //числа в начале, из за чего и сделан этот костыль
                killsText.text = playerManager.Kills.ToString();
                deathsText.text = playerManager.Deaths.ToString();
                pingText.text = playerManager.Ping.ToString();
            }
        }
    }
}
