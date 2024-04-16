using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float Sensitivity;
    public float Gravity;
    public float Speed;
    public float JumpHeight;

    private bool isGrounded;

    [SerializeField] private GameObject cam;
    private Rigidbody rg;
    private CharacterController characterController;

    private PhotonView photonView;

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

        if (isGrounded && !Input.GetKey(KeyCode.Space)) velocity = new Vector3(0, -2, 0);
        else velocity.y += Time.deltaTime * Gravity;

        characterController.Move(velocity * Time.deltaTime);
    }

    private void jump()
    {
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            velocity = new Vector3(velocity.x, Mathf.Sqrt(JumpHeight * -2 * Gravity), velocity.z);
        }
    }

    public void PushBack(float _velocity)
    {
        characterController.Move(-transform.forward.normalized * _velocity * Time.deltaTime);
    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        SetCursorVisible(false);
        look();
        jump();
        move();
    }

}
