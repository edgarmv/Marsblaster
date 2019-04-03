using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipController : MonoBehaviour
{
    public GameController controller;
    public int shipAnswer;
    public int correctAnswer;

    float t;
    Vector3 startPosition;
    Vector3 target;
    float timeToReachTarget;

    public float flashTime;
    Color origionalColor;
    public MeshRenderer renderer;

    void Start()
    {
        flashTime = 0.5f;
        renderer = GetComponent<MeshRenderer>();
        origionalColor = renderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime / timeToReachTarget;
        transform.position = Vector3.Lerp(startPosition, target, t);
    }

    public void SetDestination(Vector3 destination, float time)
    {
        t = 0;
        startPosition = transform.position;
        timeToReachTarget = time;
        target = destination;
    }

    public void rotateToPoint(Transform point)
    {
        transform.LookAt(point);
        this.gameObject.transform.GetChild(0).GetComponent<TextMesh>().transform.Rotate(new Vector3(0, 180, 0), Space.Self);
    }

    public bool isCorrect()
    {
        return shipAnswer == correctAnswer;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            controller.gotHit(isCorrect());
            FlashRed();
        }
    }

    public void FlashRed()
    {
        renderer.material.color = Color.red;
        Invoke("ResetColor", flashTime);
    }
    void ResetColor()
    {
        renderer.material.color = origionalColor;
    }
}
