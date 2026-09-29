using UnityEngine;
using TMPro;

public class ChoiceGame : MonoBehaviour
{
    public float timeLimit = 5f;
    public int winPoints = 100;

    public float resultTime = 1f;

    public string winMessage = "NICE!";
    public string loseMessage = "NOT QUITE";

    public TextMeshProUGUI timerDisplay;
    public TextMeshProUGUI messageDisplay;

    bool finished = false;
    bool reported = false;
    int pointsEarned = 0;

    void Start()
    {
        messageDisplay.text = "";
    }

    void Update()
    {
        if (finished == false)
        {
            timeLimit -= Time.deltaTime;
            timerDisplay.text = Mathf.CeilToInt(timeLimit).ToString();

            if (timeLimit <= 0)
            {
                timerDisplay.text = "0";
                Lose();
            }
        }
        else
        {
            resultTime -= Time.deltaTime;

            if (resultTime <= 0 && reported == false)
            {
                reported = true;
                ReportResult();
            }
        }
    }

    public void CorrectChoice()
    {
        if (finished == false)
        {
            pointsEarned = winPoints;
            messageDisplay.text = winMessage;
            finished = true;
        }
    }

    public void WrongChoice()
    {
        Lose();
    }

    void Lose()
    {
        if (finished == false)
        {
            messageDisplay.text = loseMessage;
            finished = true;
        }
    }

    void ReportResult()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.MinigameFinished(pointsEarned);
        }
        else
        {
            Debug.Log("Minigame over. Points: " + pointsEarned);
        }
    }
}
