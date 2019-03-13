using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipCollider : MonoBehaviour
{
    public GameController controller;
    public int shipAnswer;
    public int correctAnswer;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
        }
    }
}
