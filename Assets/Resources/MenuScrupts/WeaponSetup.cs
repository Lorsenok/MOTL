using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WeaponSetup : MonoBehaviour
{
    [SerializeField] private Slider damageSlider;
    [SerializeField] private Slider recoilSlider;
    [SerializeField] private Slider aimSlider;
    [SerializeField] private Slider sizeSlider;
    [SerializeField] private Slider reloadSlider;
    [SerializeField] private Slider ammoSlider;

    [SerializeField] private TMP_InputField redInputField;
    [SerializeField] private TMP_InputField greenInputField;
    [SerializeField] private TMP_InputField blueInputField;

    [SerializeField] private TextMeshProUGUI pointsText;

    [SerializeField] private LogText logText;

    [SerializeField] private float PointsMax = 100;
    private float points;

    public void Awake()
    {
        if (PlayerPrefs.HasKey("damage"))
        {
            WeaponData.Damage = PlayerPrefs.GetFloat("damage");
            WeaponData.Recoil = PlayerPrefs.GetFloat("recoil");
            WeaponData.Aim = PlayerPrefs.GetFloat("aim");
            WeaponData.Size = PlayerPrefs.GetFloat("size");
            WeaponData.ReloadTimeMax = PlayerPrefs.GetFloat("reload");
            WeaponData.AmmoMax = PlayerPrefs.GetFloat("ammo");
        }

        damageSlider.value = WeaponData.Damage;
        recoilSlider.value = recoilSlider.maxValue + recoilSlider.minValue - WeaponData.Recoil;
        aimSlider.value = WeaponData.Aim;
        sizeSlider.value = WeaponData.Size;
        reloadSlider.value = reloadSlider.maxValue + reloadSlider.minValue - WeaponData.ReloadTimeMax;
        ammoSlider.value = WeaponData.AmmoMax;

        redInputField.text = (WeaponData.LaserColor.r * 255).ToString();
        greenInputField.text = (WeaponData.LaserColor.g * 255).ToString();
        blueInputField.text = (WeaponData.LaserColor.b * 255).ToString();
    }

    public void Apply()
    {
        if (points > 0)
        {
            WeaponData.Damage = damageSlider.value;
            WeaponData.Recoil = recoilSlider.maxValue + recoilSlider.minValue - recoilSlider.value;
            WeaponData.Aim = aimSlider.value;
            WeaponData.Size = sizeSlider.value;
            WeaponData.ReloadTimeMax = reloadSlider.maxValue + reloadSlider.minValue - reloadSlider.value;
            WeaponData.AmmoMax = ammoSlider.value;

            PlayerPrefs.SetFloat("damage", WeaponData.Damage);
            PlayerPrefs.SetFloat("recoil", WeaponData.Recoil);
            PlayerPrefs.SetFloat("aim", WeaponData.Aim);
            PlayerPrefs.SetFloat("size", WeaponData.Size);
            PlayerPrefs.SetFloat("reload", WeaponData.ReloadTimeMax);
            PlayerPrefs.SetFloat("ammo", WeaponData.AmmoMax);

            if (redInputField.text != string.Empty & greenInputField.text != string.Empty & blueInputField.text != string.Empty & int.Parse(redInputField.text) < 256 & int.Parse(greenInputField.text) < 256 & int.Parse(blueInputField.text) < 256)
            {
                WeaponData.LaserColor = new Color(float.Parse(redInputField.text) / 255, float.Parse(greenInputField.text) / 255, float.Parse(blueInputField.text) / 255);
            }
        }
        else logText.GetComponent<LogText>().Message("Points needs to be more than 0");
    }

    public void SetToDefault()
    {
        damageSlider.value = WeaponData.DamageDefault;
        recoilSlider.value = WeaponData.RecoilDefault;
        aimSlider.value = WeaponData.AimDefault;
        sizeSlider.value = WeaponData.SizeDefault;
        reloadSlider.value = WeaponData.ReloadTimeDefault;
        ammoSlider.value = WeaponData.AmmoMaxDefault;

        redInputField.text = (WeaponData.LaserColorDefault.r * 255).ToString();
        greenInputField.text = (WeaponData.LaserColorDefault.g * 255).ToString();
        blueInputField.text = (WeaponData.LaserColorDefault.b * 255).ToString();
    }

    private void Update()
    {
        float[] values =
        {
            damageSlider.value * (100 / damageSlider.maxValue),
            recoilSlider.value * (100 / recoilSlider.maxValue),
            aimSlider.value * (100 / aimSlider.maxValue),
            sizeSlider.value * (100 / sizeSlider.maxValue),
            reloadSlider.value * (100 / reloadSlider.maxValue),
            ammoSlider.value * (100 / ammoSlider.maxValue),
        };

        points = PointsMax;
        for (int i = 0; i < values.Length; i++)
        {
            points -= values[i];
        }
        pointsText.text = "Points left: " + Mathf.Round(points).ToString();

        damageSlider.GetComponentInChildren<TextMeshProUGUI>().text = "Damage (" + damageSlider.value.ToString() + " per sec)";
        recoilSlider.GetComponentInChildren<TextMeshProUGUI>().text = "Recoil (" + (recoilSlider.maxValue + recoilSlider.minValue - recoilSlider.value).ToString() + ")";
        aimSlider.GetComponentInChildren<TextMeshProUGUI>().text = "Aim (" + aimSlider.value.ToString() + ")";
        sizeSlider.GetComponentInChildren<TextMeshProUGUI>().text = "Size (" + sizeSlider.value.ToString() + ")";
        reloadSlider.GetComponentInChildren<TextMeshProUGUI>().text = "Reload Time (" + (reloadSlider.maxValue + reloadSlider.minValue - reloadSlider.value).ToString() + " sec)";
        ammoSlider.GetComponentInChildren<TextMeshProUGUI>().text = "Ammo (" + ammoSlider.value.ToString() + " sec)";
    }
}
