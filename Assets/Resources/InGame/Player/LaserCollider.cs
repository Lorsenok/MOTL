using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCollider : MonoBehaviour
{
    [HideInInspector] public WeaponController weapon;

    private PhotonView pv;

    private void Start()
    {
        pv = GetComponentInParent<PhotonView>();
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag != "Player") return;
        if (collision.gameObject.GetComponent<PhotonView>() == null) return;
        if (collision.gameObject.GetComponent<PhotonView>().Owner == pv.Owner) return;
        PlayerController pc = collision.gameObject.GetComponentInParent<PlayerController>();
        if (pc.HP <= 0 & !pc.isDead)
        {
            if (weapon != null)
            {
                if (weapon.playerManager != null)
                {
                    weapon.playerManager.Kills += 1;
                }
            }
            pc.isDead = true;
            pc.SetDeath();
        }
        else pc.GetDamage(weapon.damage);
    }

}
