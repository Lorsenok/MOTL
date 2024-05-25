using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsSetup : MonoBehaviour
{
    private Settings settings;

    [SerializeField] private TMPro.TMP_InputField sensitivityInputField;
    [SerializeField] private Slider fovSlider;
    [SerializeField] private Toggle postProcessingToggle;

    private void Awake()
    {
        settings = FindObjectOfType<Settings>();
        if (PlayerPrefs.HasKey("sensitivity"))
        {
            sensitivityInputField.text = PlayerPrefs.GetInt("sensitivity").ToString();
            fovSlider.value = PlayerPrefs.GetFloat("fov");
            postProcessingToggle.isOn = PlayerPrefs.GetInt("postprocessing") == 1;
        }
    }

    public void Setup()
    {
        settings.Sensitivity = int.Parse(sensitivityInputField.text);
        settings.FOV = fovSlider.value;
        settings.PostProcessing = postProcessingToggle.isOn;
        PlayerPrefs.SetInt("sensitivity", settings.Sensitivity);
        PlayerPrefs.SetFloat("fov", settings.FOV);
        PlayerPrefs.SetInt("postprocessing", settings.PostProcessing? 1 : 0);
        settings.Setup();
    }

    private void Update()
    {
        fovSlider.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "FOV (" + fovSlider.value + ")";
    }
}
