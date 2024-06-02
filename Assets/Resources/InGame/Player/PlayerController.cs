using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerController : MonoBehaviour
{

    public float HP;

    public float Sensitivity;
    public float Gravity;
    public float Speed;
    public float JumpHeight;

    private bool isGrounded;

    [SerializeField] private GameObject cam;
    private Rigidbody rg;
    private CharacterController characterController;

    private PhotonView photonView;
    [HideInInspector] public PlayerManager playerManager;

    [HideInInspector] public bool canMove = true;

    [HideInInspector] public bool isDead = false;

    public float AfterDamageTimeSet = 1;
    [HideInInspector] public float AfterDamageTime = 0;

    [SerializeField] private MeshRenderer Hat;
    [SerializeField] private MeshRenderer Body;
    [SerializeField] private MeshRenderer Head;

    [SerializeField] private MeshRenderer BookCover;
    [SerializeField] private MeshRenderer BookCrust;

    [HideInInspector] public float ColorR;
    [HideInInspector] public float ColorG;
    [HideInInspector] public float ColorB;

    private void Awake()
    {
        rg = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
        characterController = GetComponent<CharacterController>();
        if (photonView.IsMine) GetComponentInChildren<Billboard>().gameObject.SetActive(false);
        else GetComponentInChildren<Billboard>().GetComponentInChildren<TextMeshProUGUI>().text = photonView.Owner.NickName;
    }

    private void Start()
    {
        if (!photonView.IsMine)
        {
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<PostProcessVolume>().enabled = false;
            Destroy(rg);
        }
        if (playerManager == null)
        {
            foreach (PlayerManager _playerManager in FindObjectsOfType<PlayerManager>())
            {
                if (_playerManager.gameObject.GetComponent<PhotonView>().Owner == photonView.Owner)
                {
                    playerManager = _playerManager;
                }
            }
        }
    }

    public void SetGroundedState(bool state)
    {
        isGrounded = state;
    }

    private float xRotation = 0f;
    private void look()
    {
        xRotation -= Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * (Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime));
    }

    private Vector3 velocity;

    private void move()
    {
        characterController.Move((transform.right * Input.GetAxis("Horizontal") + Input.GetAxis("Vertical") * transform.forward).normalized * Speed * Time.deltaTime);
    }

    private void jump()
    {
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            velocity = new Vector3(velocity.x, Mathf.Sqrt(JumpHeight * -2 * Gravity), velocity.z);
        }
    }

    public void Push(float _velocity, Vector3 direction)
    {
        characterController.Move(direction * _velocity * Time.deltaTime);
    }

    private bool once = true;
    private void Update()
    {

        Hat.materials[0].color = new Color(ColorR, ColorG, ColorB);
        Body.materials[0].color = new Color(ColorR, ColorG, ColorB);
        BookCover.materials[0].color = new Color(ColorR, ColorG, ColorB);
        BookCrust.materials[0].color = new Color(ColorR - 0.2f, ColorG - 0.2f, ColorB - 0.2f);

        Body.gameObject.SetActive(!photonView.IsMine);
        Hat.gameObject.SetActive(!photonView.IsMine);
        Head.gameObject.SetActive(!photonView.IsMine);

        if (isDead)
        {
            if (photonView.IsMine) playerManager.SpawnTimeCur = playerManager.SpawnTime;
            playerManager.player = null;
            PhotonNetwork.Destroy(gameObject);
        }

        if (!photonView.IsMine || !PhotonNetwork.InRoom || playerManager.IsLeaving) return;

        if (once) // Костыль ебаный
        {
            photonView.RPC("Setup", RpcTarget.AllBuffered, RoomData.HP, WeaponData.LaserColor.r, WeaponData.LaserColor.g, WeaponData.LaserColor.b);
            Gravity = RoomData.Gravity;
            Speed = RoomData.Speed;
            once = false;
        }

        if (AfterDamageTime > 0) AfterDamageTime -= Time.deltaTime;

        if (isGrounded && !Input.GetKey(KeyCode.Space)) velocity = new Vector3(0, -2, 0);
        else velocity.y += Time.deltaTime * Gravity;
        characterController.Move(velocity * Time.deltaTime);
        if (canMove)
        {
            look();
            jump();
            move();
        }

        if (transform.position.y < -50)
        {
             transform.position =  GameObject.FindGameObjectsWithTag("Spawner")[Random.Range(0, GameObject.FindGameObjectsWithTag("Spawner").Length - 1)].transform.position;
        }

        //photonView.RPC("Setup", RpcTarget.AllBuffered, HP);
    }

    [PunRPC]
    public void Setup(float hp, float r, float g, float b)
    {
        HP = hp;
        ColorR = r; ColorG = g; ColorB = b;
    }

    public void GetDamage(float damage)
    {
        photonView.RPC("Damage", RpcTarget.AllBuffered, damage);
        AfterDamageTime = AfterDamageTimeSet;
    }

    [PunRPC]
    public void Damage(float damage)
    {
        HP -= damage * Time.deltaTime;
    }

    public void SetDeath()
    {
        photonView.RPC("Death", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void Death()
    {
        isDead = true;
        playerManager.Deaths += 1;
    }

}