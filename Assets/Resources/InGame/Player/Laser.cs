using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    [HideInInspector] public WeaponController weapon;

    private MeshRenderer meshRenderer;
    private MeshRenderer closeMeshRenderer;

    public float OpacityMultiplier = 1;

    private void Awake()
    {
        PhotonView pv = GetComponent<PhotonView>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        foreach (WeaponController _weapon in FindObjectsOfType<WeaponController>())
        {
            if (_weapon.gameObject.GetComponent<PhotonView>().Owner == pv.Owner)
            {
                weapon = _weapon;
            }
        }

        foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            if (!meshRenderer.GetComponent<LaserCollider>())
            {
                closeMeshRenderer = meshRenderer;
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
        Vector3 direction = meshRenderer.gameObject.transform.position - weapon.LaserSpawnPoint.transform.position;
        Ray ray = new(weapon.LaserSpawnPoint.transform.position, direction.normalized);
        Physics.Raycast(ray, out hit, Mathf.Infinity, weapon.CheckRaycastLayer);

        float distance;

        if (hit.collider != null) distance = Vector3.Distance(transform.position, hit.point);
        else distance = weapon.DistanceWithoutObstacles;

        meshRenderer.gameObject.transform.localScale = new Vector3(weapon.Size, distance / 2 - weapon.LaserSizeCut, weapon.Size);
        meshRenderer.gameObject.transform.localPosition = new Vector3(0, distance / 2, 0);

        meshRenderer.materials[0].mainTextureScale = new Vector2(1, meshRenderer.transform.localScale.y);

        meshRenderer.materials[0].color = new Color(meshRenderer.materials[0].color.r, meshRenderer.materials[0].color.g, meshRenderer.materials[0].color.b, (255 - OpacityMultiplier * weapon.Size) / 255);
        closeMeshRenderer.materials[0].color = new Color(meshRenderer.materials[0].color.r, meshRenderer.materials[0].color.g, meshRenderer.materials[0].color.b, (255 - OpacityMultiplier * weapon.Size * 2) / 255);
    }
}
