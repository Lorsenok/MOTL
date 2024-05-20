using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Laser : MonoBehaviour
{

    [HideInInspector] public WeaponController weapon;

    private void Awake()
    {
        PhotonView pv = GetComponent<PhotonView>();

        foreach (WeaponController _weapon in FindObjectsOfType<WeaponController>())
        {
            if (_weapon.gameObject.GetComponent<PhotonView>().Owner == pv.Owner)
            {
                weapon = _weapon;
            }
        }

        GetComponentInChildren<LaserCollider>().weapon = weapon;
    }

    private RaycastHit hit;
    private void Update()
    {
        if (weapon == null)
        {
            Destroy(gameObject);
            return;
        }
        Vector3 direction = GetComponentInChildren<MeshRenderer>().gameObject.transform.position - weapon.LaserSpawnPoint.transform.position;
        Ray ray = new(weapon.LaserSpawnPoint.transform.position, direction.normalized);
        Physics.Raycast(ray, out hit, Mathf.Infinity, weapon.CheckRaycastLayer);

        float distance;

        if (hit.collider != null) distance = Vector3.Distance(transform.position, hit.point);
        else distance = weapon.DistanceWithoutObstacles;

        GetComponentInChildren<MeshRenderer>().gameObject.transform.localScale = new Vector3(weapon.Size, distance / 2 - weapon.LaserSizeCut, weapon.Size);
        GetComponentInChildren<MeshRenderer>().gameObject.transform.localPosition = new Vector3(0, distance / 2, 0);
    }
}
