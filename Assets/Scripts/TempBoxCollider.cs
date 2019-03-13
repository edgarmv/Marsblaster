using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempBoxCollider : MonoBehaviour
{

    public Text scoreBoard;
    private int score = 1;

    // Start is called before the first frame update
    void Start()
    {
        scoreBoard.text = "Score: " + score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Bullet")
        {
            score++;
            scoreBoard.text = "Score: " + score.ToString();
            // Destroy(collision.gameObject);
        }
    }
}
