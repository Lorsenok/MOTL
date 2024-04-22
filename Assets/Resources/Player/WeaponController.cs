using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class WeaponController : MonoBehaviour
{
    
    [SerializeField] private GameObject laserSpawnPoint;
    [SerializeField] private LayerMask checkRaycastLayer;
    [SerializeField] private float distanceWithoutObstacles;
    [SerializeField] private float laserSizeCut;

    [SerializeField] private float shootDelaySet;
    private float shootDelay = 0;

    //На релизе все SerializeField ниже удалить!!!
    [SerializeField] private Color laserColor;

    [SerializeField] private float damage;
    [SerializeField] private float recoil;
    [SerializeField] private float aim;
    [SerializeField] private float size;

    [SerializeField] private float reloadTimeMax;
    private float reloadTime = 0;
    private bool isReloading = false;

    [SerializeField] private float ammoMax;
    [SerializeField] private float ammo;

    private PhotonView photonView;

    public void Setup()
    {
        damage = WeaponData.Damage;
        recoil = WeaponData.Recoil;
        aim = WeaponData.Aim;
        size = WeaponData.Size;

        reloadTimeMax = WeaponData.ReloadTimeMax;
        reloadTime = reloadTimeMax;

        ammoMax = WeaponData.AmmoMax;
        ammo = ammoMax;

        laserColor = WeaponData.LaserColor;
    }

    private RaycastHit hit;
    private GameObject laser;
    private void shoot()
    {
        if (shootDelay > 0)
        {
            shootDelay -= Time.deltaTime;
        }

        if (Input.GetMouseButton(0) & ammo > 0 & !isReloading & shootDelay <= 0)
        {
            if (laser == null)
            {
                laser = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Laser"), laserSpawnPoint.transform.position, laserSpawnPoint.transform.rotation);
                laser.GetComponentInChildren<MeshRenderer>().material.color = laserColor;
            }

            ammo -= Time.deltaTime;

            laser.transform.position = laserSpawnPoint.transform.position;
            laser.transform.rotation = Quaternion.RotateTowards(laser.transform.rotation, laserSpawnPoint.transform.rotation, aim * Time.deltaTime);
            GetComponent<PlayerController>().Push(Time.deltaTime * recoil, -transform.forward.normalized);

            Vector3 direction = laser.GetComponentInChildren<MeshRenderer>().gameObject.transform.position - laserSpawnPoint.transform.position;
            Ray ray = new(laserSpawnPoint.transform.position, direction.normalized);
            Physics.Raycast(ray, out hit, Mathf.Infinity, checkRaycastLayer);

            float distance;

            if (hit.collider != null) distance = Vector3.Distance(transform.position, hit.point);
            else distance = distanceWithoutObstacles;

            laser.GetComponentInChildren<MeshRenderer>().gameObject.transform.localScale = new Vector3(size, distance / 2 - laserSizeCut, size);
            laser.GetComponentInChildren<MeshRenderer>().gameObject.transform.localPosition = new Vector3(0, distance / 2, 0);

        }
        else
        {
            if (laser != null)
            {
                PhotonNetwork.Destroy(laser);
                laser = null;
                shootDelay = shootDelaySet;
            }
        }
    }

    private void reload()
    {
        if (ammo <= 0 | Input.GetKeyDown(KeyCode.R))
        {
            if (!isReloading)
            {
                reloadTime = reloadTimeMax;
                isReloading = true;
            }
        }

        if (isReloading)
        {
            reloadTime -= Time.deltaTime;
            if (reloadTime <= 0)
            {
                isReloading = false;
                ammo = ammoMax;
            }
        }
    }

    private void Start()
    {
        Setup();
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        foreach (AudioListener audio in FindObjectsOfType<AudioListener>())
        {
            if (audio != GetComponent<AudioListener>()) Destroy(audio);
        }

        if (GetComponent<PlayerController>().canMove)
        {
            shoot();
        }
        reload();
    }

}
