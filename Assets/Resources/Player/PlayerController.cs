using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] public float sensitivity;
    [SerializeField] public float gravity;
    [SerializeField] public float speed;
    [SerializeField] public float jumpHeight;

    private bool isGrounded;

    private GameObject cam;
    private Rigidbody rg;
    private CharacterController characterController;

    private PhotonView photonView;

    private void Awake()
    {
        rg = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
        cam = GetComponentInChildren<Camera>().gameObject;
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
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SetGroundedState(bool state)
    {
        isGrounded = state;
    }

    private float xRotation = 0f;
    private void look()
    {
        xRotation -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * (Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime));
    }

    [SerializeField] private Vector3 velocity;
    private void move()
    {
        characterController.Move((transform.right * Input.GetAxis("Horizontal") + Input.GetAxis("Vertical") * transform.forward) * speed * Time.deltaTime);

        if (isGrounded && !Input.GetKey(KeyCode.Space)) velocity = new Vector3(0, -2, 0);
        else velocity.y += Time.deltaTime * gravity;

        characterController.Move(velocity * Time.deltaTime);
    }

    private void jump()
    {
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            velocity = new Vector3(velocity.x, Mathf.Sqrt(jumpHeight * -2 * gravity), velocity.z);
        }
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
