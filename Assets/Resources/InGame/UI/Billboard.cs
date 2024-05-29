using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private GameObject cam;

    private void Update()
    {
        if (cam == null)
        {
            foreach (GameObject _cam in GameObject.FindGameObjectsWithTag("Camera"))
            {
                if (_cam.GetComponentInParent<PlayerController>().GetComponent<PhotonView>().IsMine) cam = _cam.gameObject;
            }
        }
        if (cam == null) return;

        transform.LookAt(cam.transform);
        transform.Rotate(Vector3.up * 180);
    }
}
