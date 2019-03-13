using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ExtensionMethods;
using UnityEngine.UI;

public struct QuestionData
{
    public string question;
    public List<int> alternatives;
}

public class GameController : MonoBehaviour
{
    public TextMesh Question;
    public TextMesh Alternative1;
    public TextMesh Alternative2;
    public TextMesh Alternative3;

    public GameObject AlienShip;
    public Text scoreBoard;
    private int score = 0;
    private GameObject ship1;
    private GameObject ship2;
    private GameObject ship3;
    private int correctAnswer;
    
    private static System.Random randomGenerator = new System.Random();

    void Start()
    {
        scoreBoard.text = "Score: " + score.ToString();
        StartGame();
    }

    void StartGame()
    {
        SpawnShips();
        CreateNewQuestion();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void CreateNewQuestion() {
        QuestionData questionData = GenerateMultiplicationQuestion(3); 
        Question.text = questionData.question;
        Alternative1.text = String.Format("{0}", questionData.alternatives[0]);
        ship1.GetComponent<ShipCollider>().shipAnswer = questionData.alternatives[0];
        ship1.GetComponent<ShipCollider>().correctAnswer = correctAnswer;

        Alternative2.text = String.Format("{0}", questionData.alternatives[1]);
        ship2.GetComponent<ShipCollider>().shipAnswer = questionData.alternatives[1];
        ship2.GetComponent<ShipCollider>().correctAnswer = correctAnswer;

        Alternative3.text = String.Format("{0}", questionData.alternatives[2]);
        ship3.GetComponent<ShipCollider>().shipAnswer = questionData.alternatives[2];
        ship3.GetComponent<ShipCollider>().correctAnswer = correctAnswer;
    }

    /* Returns true or false with 50/50 probability */
    bool randomBool() {
        bool res = randomGenerator.Next(100) < 50 ? true : false;
        Debug.Log(res);
        return res;
    }

    /* Generates a list of nearby digits, clamped between 1 and 9 inclusive */
    List<int> GenerateNearbyDigits(int digit, int count) {
        List<int> digits = new List<int>();
        int difference = count / 2;
        int min = digit - difference - (count % 2);
        int max = digit + difference;
        for (int i = min; i <= max; i++) {
            if (i == digit) {
                continue;
            }
            if (i <= 0) {
                digits.Add(i + count + 1);
            } else if (i >= 10) {
                digits.Add(i - count - 1);
            } else {
                digits.Add(i);
            }
        }

        digits.Shuffle();

        return digits;
    }


    QuestionData GenerateMultiplicationQuestion(int numAlternatives) {
        QuestionData questionData = new QuestionData();
        System.Random randomGenerator = new System.Random();
        int firstDigit = randomGenerator.Next(2, 10);
        int secondDigit = randomGenerator.Next(2, 10);
        List<int> alternatives = new List<int>();
        alternatives.Add(firstDigit * secondDigit);
        correctAnswer = firstDigit * secondDigit;


        List<int> nearbyDigits;
        int otherDigit;
        if (randomBool()) {
            nearbyDigits = GenerateNearbyDigits(firstDigit, numAlternatives - 1);
            otherDigit = secondDigit;

        } else {
            nearbyDigits = GenerateNearbyDigits(secondDigit, numAlternatives - 1);
            otherDigit = firstDigit;
        }

        foreach (int digit in nearbyDigits) {
            alternatives.Add(digit * otherDigit);
        }

        alternatives.Shuffle();

        questionData.question = String.Format("{0} * {1}", firstDigit, secondDigit);
        questionData.alternatives = alternatives;

        return questionData;
    }

    public void gotHit(bool hit){
        if (hit){
            score++;
            scoreBoard.text = "Score: " + score.ToString();
        }
        Destroy(ship1);
        Destroy(ship2);
        Destroy(ship3);
    }

    public void SpawnShips()
    {
        ship1 = Instantiate(AlienShip);
        ship1.transform.Translate(Vector3.forward * 10);
        ship1.GetComponent<ShipCollider>().controller = this;
        Alternative1 = ship1.gameObject.transform.GetChild(0).GetComponent<TextMesh>();

        ship2 = Instantiate(AlienShip);
        ship2.transform.Translate(Vector3.forward * 10);
        ship2.transform.Translate(Vector3.left * 10);
        ship2.GetComponent<ShipCollider>().controller = this;
        Alternative2 = ship2.gameObject.transform.GetChild(0).GetComponent<TextMesh>();

        ship3 = Instantiate(AlienShip);
        ship3.transform.Translate(Vector3.forward * 10);
        ship3.transform.Translate(Vector3.right * 10);
        ship3.GetComponent<ShipCollider>().controller = this;
        Alternative3 = ship3.gameObject.transform.GetChild(0).GetComponent<TextMesh>();
    }
}

namespace ExtensionMethods
{
    public static class Extension
    {
        public static void Shuffle<T>(this IList<T> list) {
            System.Random randomGenerator = new System.Random();
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = randomGenerator.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
