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
                Debug.Log(collision.gameObject.name);
                collision.gameObject.GetComponent<PlayerController>().GetDamage(weapon.damage);
            }
        }
    }

}
