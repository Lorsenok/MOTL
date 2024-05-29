using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameInterface : MonoBehaviour
{
    [SerializeField] private PlayerManager pm;
    private WeaponController wc;

    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private RectTransform reloadBar;
    [SerializeField] private Image damage;

    private void Update()
    {
        if (pm.player == null) return;

        if (wc == null)
        {
            wc = pm.player.GetComponent<WeaponController>();
            return;
        }

        hpText.text = Math.Round(pm.player.HP).ToString();
        if (wc.Ammo > 0) ammoText.text = Math.Round(wc.Ammo, 2).ToString();
        else ammoText.text = "0";

        float reloadBarScale = 1 - 1 * wc.ReloadTime / wc.ReloadTimeMax;
        reloadBar.gameObject.SetActive(reloadBarScale < 1);
        reloadBar.localScale = new Vector3(reloadBarScale, reloadBarScale, reloadBarScale);
        damage.color = new(1, 1, 1, pm.player.AfterDamageTime);
    }
}
