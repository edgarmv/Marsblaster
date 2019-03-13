using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletController : MonoBehaviour
{
    public float projectileSpeed;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        projectileSpeed = 15;
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = this.transform.forward * projectileSpeed;
        // Debug.Log(rb.velocity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this);
    }
}
