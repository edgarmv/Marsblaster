using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ExtensionMethods;
using UnityEngine.UI;
using System.Timers;

public struct QuestionData
{
    public string question;
    public List<int> alternatives;
}

public class GameController : MonoBehaviour
{
    private TextMesh Alternative1;
    private TextMesh Alternative2;
    private TextMesh Alternative3;
    public TextMesh Question;
    public TextMesh scoreBoard;
    public TextMesh timeText;
    public static TextMesh splashText;

    private GameObject ship1;
    private GameObject ship2;
    private GameObject ship3;
    public GameObject AlienShip;
    public GameObject spawnPoint1;
    public GameObject spawnPoint2;
    public GameObject spawnPoint3;
    public GameObject movePoint1;
    public GameObject movePoint2;
    public GameObject movePoint3;
    public GameObject leavePoint1;
    public GameObject leavePoint2;
    public GameObject leavePoint3;

    private int correctAnswer;
    private int score = 0;
    private static int scoreMultiplier = 3;
    private static int roundTime;

    private static Timer shipTimer = new System.Timers.Timer();
    private static Timer scoreTimer = new System.Timers.Timer();
    private static Timer splashTimer = new System.Timers.Timer();
    private static Timer gameTimer = new System.Timers.Timer();

    private static Boolean shipSpawn = false;
    
    private static System.Random randomGenerator = new System.Random();

    void Start()
    {
        scoreBoard.text = "Score: " + score.ToString();
        StartGame();
        shipTimer.Elapsed += new ElapsedEventHandler(OnShipTimedEvent);
        shipTimer.Interval = 5000;

        scoreTimer.Elapsed += new ElapsedEventHandler(OnScoreTimedEvent);
        scoreTimer.Interval = 10000;

        splashTimer.Elapsed += new ElapsedEventHandler(OnSplashTimedEvent);
        splashTimer.Interval = 1000;

        gameTimer.Elapsed += new ElapsedEventHandler(OnGameTimedEvent);
        gameTimer.Interval = 1000;
        gameTimer.Enabled = true;

        roundTime = 180;
        splashText = GameObject.Find("SplashText").GetComponent<TextMesh>();
    }

    private static void OnShipTimedEvent(object source, ElapsedEventArgs e)
    {
        shipSpawn = true;
    }

    private static void OnScoreTimedEvent(object source, ElapsedEventArgs e)
    {
        if(scoreMultiplier > 1)
        {
            scoreMultiplier--;
        }
        else
        {
            scoreTimer.Enabled = false;
        }
    }

    private static void OnSplashTimedEvent(object source, ElapsedEventArgs e)
    {
        splashText.text = "";
        splashTimer.Enabled = false;
    }

    private static void OnGameTimedEvent(object source, ElapsedEventArgs e)
    {
        if (roundTime > 0)
        {
            roundTime--;
        }
        else
        {
            gameTimer.Enabled = false;
        }
    }

    void StartGame()
    {
        SpawnShips();
        CreateNewQuestion();
        shipTimer.Enabled = false;
        scoreTimer.Enabled = true;
        scoreMultiplier = 3;
    }

    // Update is called once per frame
    void Update()
    {
        if (shipSpawn && roundTime > 0)
        {
            StartGame();
            shipSpawn = false;
        }
        if(ship1 != null && roundTime == 0)
        {
            RemoveShips();
        }
        timeText.text = "Time: " + roundTime;

    }

    void CreateNewQuestion() {
        QuestionData questionData = GenerateMultiplicationQuestion(3); 
        Question.text = questionData.question;
        Alternative1.text = String.Format("{0}", questionData.alternatives[0]);
        ship1.GetComponent<ShipController>().shipAnswer = questionData.alternatives[0];
        ship1.GetComponent<ShipController>().correctAnswer = correctAnswer;

        Alternative2.text = String.Format("{0}", questionData.alternatives[1]);
        ship2.GetComponent<ShipController>().shipAnswer = questionData.alternatives[1];
        ship2.GetComponent<ShipController>().correctAnswer = correctAnswer;

        Alternative3.text = String.Format("{0}", questionData.alternatives[2]);
        ship3.GetComponent<ShipController>().shipAnswer = questionData.alternatives[2];
        ship3.GetComponent<ShipController>().correctAnswer = correctAnswer;
    }

    /* Returns true or false with 50/50 probability */
    bool randomBool() {
        bool res = randomGenerator.Next(100) < 50 ? true : false;
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
            score += 5*scoreMultiplier;
            splashTimer.Enabled = true;
            scoreBoard.text = "Score: " + score;
            splashText.text = "+ " + (5 * scoreMultiplier);
        }

        RemoveShips();

        shipTimer.Enabled = true;
        scoreTimer.Enabled = false;
    }

    public void SpawnShips()
    {
        ship1 = Instantiate(AlienShip, spawnPoint1.transform);
        ship1.GetComponent<ShipController>().controller = this;
        Alternative1 = ship1.gameObject.transform.GetChild(0).GetComponent<TextMesh>();

        ship2 = Instantiate(AlienShip, spawnPoint2.transform);
        ship2.GetComponent<ShipController>().controller = this;
        Alternative2 = ship2.gameObject.transform.GetChild(0).GetComponent<TextMesh>();

        ship3 = Instantiate(AlienShip, spawnPoint3.transform);
        ship3.GetComponent<ShipController>().controller = this;
        Alternative3 = ship3.gameObject.transform.GetChild(0).GetComponent<TextMesh>();

        moveShip(ship1, movePoint1, 10f);
        moveShip(ship2, movePoint2, 10f);
        moveShip(ship3, movePoint3, 10f);
    }

    public void RemoveShips()
    {
        moveShip(ship1, leavePoint1, 5f);
        moveShip(ship2, leavePoint2, 5f);
        moveShip(ship3, leavePoint3, 5f);

        Alternative1.text = "";
        Alternative2.text = "";
        Alternative3.text = "";

        ship1.GetComponent<Collider>().enabled = false;
        ship2.GetComponent<Collider>().enabled = false;
        ship3.GetComponent<Collider>().enabled = false;

        Destroy(ship1, 5);
        Destroy(ship2, 5);
        Destroy(ship3, 5);
    }

    public void moveShip(GameObject ship, GameObject point, float time)
    {
        ship.GetComponent<ShipController>().rotateToPoint(point.transform);
        ship.GetComponent<ShipController>().SetDestination(point.transform.position, time);
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
