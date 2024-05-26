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

    public float ReloadTimeMax { get; private set; }
    public float ReloadTime { get; set; }

    private bool isReloading = false;

    public float AmmoMax { get; private set; }
    public float Ammo { get; private set; }

    private PhotonView photonView;
    [HideInInspector] public PlayerManager playerManager;

    [PunRPC] public void Setup(float _damage, float _recoil, float _aim, float _size, float _reloadTimeMax, float _ammoMax, float r, float g, float b)
    {
        damage = _damage;
        recoil = _recoil;
        aim = _aim;
        Size = _size;
        ReloadTimeMax = _reloadTimeMax;
        AmmoMax = _ammoMax;
        laserColor = new Color(r, g, b);
    }

    private GameObject laser;
    private PlayerController playerController;
    private void shoot()
    {
        if (shootDelay > 0)
        {
            shootDelay -= Time.deltaTime;
        }

        if (Input.GetMouseButton(0) & Ammo > 0 & !isReloading & shootDelay <= 0)
        {
            if (laser == null)
            {
                laser = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Laser"), LaserSpawnPoint.transform.position, LaserSpawnPoint.transform.rotation);
                laser.GetComponentInChildren<MeshRenderer>().material.color = laserColor;
            }

            Ammo -= Time.deltaTime;

            laser.transform.position = LaserSpawnPoint.transform.position;
            laser.transform.rotation = Quaternion.RotateTowards(laser.transform.rotation, LaserSpawnPoint.transform.rotation, aim * Time.deltaTime);
            playerController.Push(Time.deltaTime * recoil, -transform.forward);
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
        if (Ammo <= 0 | Input.GetKeyDown(KeyCode.R))
        {
            if (!isReloading)
            {
                ReloadTime = ReloadTimeMax;
                isReloading = true;
            }
        }

        if (isReloading)
        {
            ReloadTime -= Time.deltaTime;
            if (ReloadTime <= 0)
            {
                isReloading = false;
                Ammo = AmmoMax;
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

        if (playerController.canMove)
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
        playerController = GetComponent<PlayerController>();
        ReloadTime = 0;
    }

}
