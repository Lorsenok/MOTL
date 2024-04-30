using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class WeaponController : MonoBehaviour
{
    
    public GameObject LaserSpawnPoint;
    public LayerMask CheckRaycastLayer;
    public float DistanceWithoutObstacles;
    public float LaserSizeCut;

    [SerializeField] private float shootDelaySet;
    private float shootDelay = 0;

    //На релизе все SerializeField ниже удалить!!!
    [SerializeField] private Color laserColor;

    public float damage;
    [SerializeField] private float recoil;
    [SerializeField] private float aim;
    public float Size;

    [SerializeField] private float reloadTimeMax;
    private float reloadTime = 0;
    private bool isReloading = false;

    [SerializeField] private float ammoMax;
    [SerializeField] private float ammo;

    private PhotonView photonView;
    [HideInInspector] public PlayerManager playerManager;

    [PunRPC] public void Setup(float _damage, float _recoil, float _aim, float _size, float _reloadTimeMax, float _ammoMax, float r, float g, float b)
    {
        damage = _damage;
        recoil = _recoil;
        aim = _aim;
        Size = _size;
        reloadTimeMax = _reloadTimeMax;
        ammoMax = _ammoMax;
        laserColor = new Color(r, g, b);
    }

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
                laser = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Laser"), LaserSpawnPoint.transform.position, LaserSpawnPoint.transform.rotation);
                laser.GetComponentInChildren<MeshRenderer>().material.color = laserColor;
            }

            ammo -= Time.deltaTime;

            laser.transform.position = LaserSpawnPoint.transform.position;
            laser.transform.rotation = Quaternion.RotateTowards(laser.transform.rotation, LaserSpawnPoint.transform.rotation, aim * Time.deltaTime);
            GetComponent<PlayerController>().Push(Time.deltaTime * recoil, -transform.forward.normalized);

            /*
            Vector3 direction = laser.GetComponentInChildren<MeshRenderer>().gameObject.transform.position - laserSpawnPoint.transform.position;
            Ray ray = new(laserSpawnPoint.transform.position, direction.normalized);
            Physics.Raycast(ray, out hit, Mathf.Infinity, checkRaycastLayer);

            float distance;

            if (hit.collider != null) distance = Vector3.Distance(transform.position, hit.point);
            else distance = distanceWithoutObstacles;

            laser.GetComponentInChildren<MeshRenderer>().gameObject.transform.localScale = new Vector3(size, distance / 2 - laserSizeCut, size);
            laser.GetComponentInChildren<MeshRenderer>().gameObject.transform.localPosition = new Vector3(0, distance / 2, 0);
            */
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

    private void Update()
    {
        if (!photonView.IsMine | !PhotonNetwork.InRoom) return;
        if (playerManager.IsLeaving) return;

        photonView.RPC("Setup", RpcTarget.AllBuffered, WeaponData.Damage, WeaponData.Recoil, WeaponData.Aim, WeaponData.Size, WeaponData.ReloadTimeMax, WeaponData.AmmoMax, WeaponData.LaserColor.r, WeaponData.LaserColor.g, WeaponData.LaserColor.b);

        foreach (AudioListener audio in FindObjectsOfType<AudioListener>())
        {
            if (audio != GetComponent<AudioListener>()) Destroy(audio);
        }

        if (GetComponent<PlayerController>().canMove)
        {
            shoot();
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
        reload();
    }

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

}
