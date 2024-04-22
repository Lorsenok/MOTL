using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomSetup : MonoBehaviour
{
    [SerializeField] private Slider playTimeSlider;
    [SerializeField] private Slider spawnTimeSlider;
    [SerializeField] private Slider gravitySlider;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider speedSlider;
    [SerializeField] private Slider damageMultiplierSlider;

    private void Start()
    {
        playTimeSlider.value = RoomData.PlayTimeDefault;
        spawnTimeSlider.value = RoomData.SpawnTimeDefault;
        gravitySlider.value = RoomData.GravityDefault;
        hpSlider.value = RoomData.HPDefault;
        speedSlider.value = RoomData.SpeedDefault;
        damageMultiplierSlider.value = RoomData.DamageMultiplierDefault;
    }

    private void Update()
    {
        RoomData.PlayTime = playTimeSlider.value;
        RoomData.SpawnTime = spawnTimeSlider.value;
        RoomData.Gravity = gravitySlider.value;
        RoomData.HP = hpSlider.value;
        RoomData.Speed = speedSlider.value;
        RoomData.DamageMultiplier = damageMultiplierSlider.value;

        playTimeSlider.GetComponentInChildren<TextMeshProUGUI>().text = "Play time (" + Mathf.Round(playTimeSlider.value).ToString() + " min)";
        spawnTimeSlider.GetComponentInChildren<TextMeshProUGUI>().text = "Spawn time (" + Mathf.Round(spawnTimeSlider.value).ToString() + " sec)";
        gravitySlider.GetComponentInChildren<TextMeshProUGUI>().text = "Gravity (" + Mathf.Round(gravitySlider.value).ToString() + ")";
        hpSlider.GetComponentInChildren<TextMeshProUGUI>().text = "HP (" + Mathf.Round(hpSlider.value).ToString() + ")";
        speedSlider.GetComponentInChildren<TextMeshProUGUI>().text = "Speed (" + Mathf.Round(speedSlider.value).ToString() + ")";
        damageMultiplierSlider.GetComponentInChildren<TextMeshProUGUI>().text = "Damage Multiplier (" + Mathf.Round(damageMultiplierSlider.value).ToString() + ")";
    }
}
