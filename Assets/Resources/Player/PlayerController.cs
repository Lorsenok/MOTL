using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private GameObject cameraHolder;

    [SerializeField] private float sensitivity;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float smoothTime;

    [SerializeField] private bool isGrounded;

    private float verticalLookRotation;

    private Vector3 smoothMoveVelocity;
    private Vector3 moveAmount;

    private Rigidbody rg;
    private PhotonView photonView;

    private void Awake()
    {
        rg = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (!photonView.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>());
            Destroy(rg);
        }
        SetCursorVisible(false);
    }

    public void SetCursorVisible(bool visible)
    {
        Cursor.visible = visible;
    }

    public void SetGroundedState(bool state)
    {
        isGrounded = state;
    }

    private void look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * sensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * sensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    private void move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        moveAmount = Vector3.Lerp(moveAmount, moveDir * speed, smoothTime*Time.deltaTime);
    }

    private void jump()
    {
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            rg.AddForce(transform.up * jumpForce);
        }
    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        look();
        move();
        jump();
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        rg.MovePosition(rg.position + transform.TransformDirection(moveAmount) * Time.deltaTime);
    }

}
