using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Death : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private TMP_Text text;

    private PlayerManager playerManager;

    private void Awake()
    {
        playerManager = GetComponentInParent<PlayerManager>();
    }

    public void Work()
    {
        if (playerManager.SpawnTimeCur <= 0)
        {
            playerManager.StartGame();
            Debug.Log("+");
        }
    }

    private void Update()
    {
        menu.SetActive(playerManager.player == null);
        if (playerManager.player != null) return;
        if (playerManager.SpawnTimeCur > 0)
        {
            text.text = "Wait " + Mathf.Round(playerManager.SpawnTimeCur).ToString() + "s";
        }
        else
        {
            text.text = "Respawn";
        }
    }
}
