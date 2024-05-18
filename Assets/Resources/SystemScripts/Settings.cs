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
    }

    public void Setup()
    {
        if (FindObjectOfType<PlayerController>())
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            player.Sensitivity = Sensitivity;
            player.GetComponentInChildren<Camera>().fieldOfView = FOV;
        }
    }
}
