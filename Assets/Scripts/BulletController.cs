using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletController : MonoBehaviour
{
    public float projectileSpeed;

    // Start is called before the first frame update
    void Start()
    {
        projectileSpeed = 10;
    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody rb = this.GetComponent<Rigidbody>();
        rb.velocity = this.transform.forward * projectileSpeed;
        // Debug.Log(rb.velocity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this);
    }
}
