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
            if (!collision.gameObject.GetComponent<PhotonView>().IsMine)
            {
                PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
                if (pc.HP <= 0 & !pc.isDead)
                {
                    weapon.playerManager.Kills += 1;
                    pc.isDead = true;
                    pc.SetDeath();
                }
                else pc.GetDamage(weapon.damage);
            }
        }
    }

}
