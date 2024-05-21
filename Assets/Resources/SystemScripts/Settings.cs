using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public int Sensitivity;
    public float FOV;
    public bool PostProcessing;

    private void Start()
    {
        foreach (Settings obj in FindObjectsOfType<Settings>())
        {
            if (obj != this) Destroy(obj);
        }
        DontDestroyOnLoad(gameObject);
        if (PlayerPrefs.HasKey("sensitivity"))
        {
            Sensitivity = PlayerPrefs.GetInt("sensitivity");
            FOV = PlayerPrefs.GetFloat("fov");
            PostProcessing = PlayerPrefs.GetInt("postprocessing") == 1;
        }
        else FOV = 60f;
    }

    public void Setup()
    {
        foreach (PlayerController player in FindObjectsOfType<PlayerController>())
        {
            if (player.GetComponent<PhotonView>().IsMine)
            {
                player.Sensitivity = Sensitivity;
                player.GetComponentInChildren<Camera>().fieldOfView = FOV;
            }
        }
    }
}
