using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        rg = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (!photonView.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>());
            Destroy(rg);
        }
    }

    public void SetCursorVisible(bool visible)
    {
        Cursor.visible = visible;
        if (visible) Cursor.lockState = CursorLockMode.None;
        if (!visible) Cursor.lockState = CursorLockMode.Locked;
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
        characterController.Move((transform.right * Input.GetAxis("Horizontal") + Input.GetAxis("Vertical") * transform.forward) * Speed * Time.deltaTime);
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
        if (HP <= 0 & !once)
        {
            playerManager.StartGame();
            PhotonNetwork.Destroy(gameObject);
        }

        if (!photonView.IsMine | !PhotonNetwork.InRoom) return;
        if (playerManager.isLeaving) return;

        if (once) // Костыль ебаный
        {
            photonView.RPC("Setup", RpcTarget.AllBuffered, RoomData.HP);
            Gravity = RoomData.Gravity;
            Speed = RoomData.Speed;
            once = false;
        }

        if (isGrounded && !Input.GetKey(KeyCode.Space)) velocity = new Vector3(0, -2, 0);
        else velocity.y += Time.deltaTime * Gravity;
        characterController.Move(velocity * Time.deltaTime);
        if (canMove)
        {
            look();
            jump();
            move();
        }
        //photonView.RPC("Setup", RpcTarget.AllBuffered, HP);
    }

    [PunRPC] public void Setup(float hp)
    {
        HP = hp;
    }

    public void GetDamage(float damage)
    {
        photonView.RPC("Damage", RpcTarget.AllBuffered, damage);
    }

    [PunRPC] public void Damage(float damage)
    {
        HP -= damage * Time.deltaTime;
    }

}
