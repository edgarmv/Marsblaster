using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{

    public GameObject projectilePrefab;

    // Use this for initialization
    void Start()
    {
        this.transform.forward = Camera.main.transform.forward;
    }

    public void ShootProjectile(float projectileSpeed)
    {
        GameObject projectile = Instantiate(projectilePrefab);
        projectile.transform.position = this.transform.position;

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = this.transform.forward * projectileSpeed;
    }
}