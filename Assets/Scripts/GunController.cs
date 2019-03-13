using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{

    public GameObject projectilePrefab;

    // Use this for initialization
    void Start()
    {      
    }

    public void ShootProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab);
        projectile.transform.position = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTrackedRemote);
        projectile.transform.Translate(Vector3.up * 1.72f);
        projectile.transform.rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTrackedRemote);
        // Debug.Log(OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTrackedRemote));
    }
}