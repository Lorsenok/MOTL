using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCollider : MonoBehaviour
{
    [HideInInspector] public WeaponController weapon;

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
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

}
