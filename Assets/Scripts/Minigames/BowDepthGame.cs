using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class BowDepthGame : MonoBehaviour
{
    public InputAction selectAction;

    public string formalPrompt = "GREET YOUR GRANDFATHER";
    public string casualPrompt = "GREET YOUR TEAMMATE";

    public Transform marker;
    public float meterHeight = 400f;
    public float meterSpeed = 1.2f;

    public float formalMin = 0.7f;
    public float formalMax = 0.95f;
    public float casualMin = 0.1f;
    public float casualMax = 0.3f;

    public float timeLimit = 5f;
    public int basePoints = 50;
    public int speedBonus = 50;
    public float resultTime = 1f;

    public List<string> formalWinLines = new()
    {
        "GRANDPA IS PLEASED",
        "BEAUTIFUL BOW!",
        "SO RESPECTFUL!"
    };

    public List<string> casualWinLines = new()
    {
        "COOL NOD!",
        "SMOOTH, NOT WEIRD",
        "TEAMMATE APPROVED"
    };

    public List<string> tooFormalLines = new()
    {
        "IT'S JUST YOUR TEAMMATE...",
        "WHY ARE YOU BOWING SO LOW?",
        "YOUR TEAM IS CONFUSED"
    };

    public List<string> tooCasualLines = new()
    {
        "GRANDPA IS NOT IMPRESSED",
        "A NOD? FOR GRANDPA?",
        "LOWER! LOWER!"
    };

    public List<string> missLines = new()
    {
        "WHAT WAS THAT?",
        "DID YOU TRIP?",
        "HALF A BOW?"
    };

    public List<string> tooSlowLines = new()
    {
        "FROZEN?",
        "SAY HI!",
        "THEY'RE STILL WAITING..."
    };

    public TextMeshProUGUI promptDisplay;
    public TextMeshProUGUI timerDisplay;
    public Host host;

    bool isFormal;
    float meterValue = 0;
    float meterDirection = 1;

    bool finished = false;
    bool reported = false;
    int pointsEarned = 0;
    float totalTime;

    private void OnEnable()
    {
        selectAction.Enable();
    }

    private void OnDisable()
    {
        selectAction.Disable();
    }

    void Start()
    {
        totalTime = timeLimit;

        if (Random.Range(0, 2) == 0)
        {
            isFormal = true;
            promptDisplay.text = formalPrompt;
        }
        else
        {
            isFormal = false;
            promptDisplay.text = casualPrompt;
        }

        MoveMarker();
    }

    void Update()
    {
        if (finished == false)
        {
            timeLimit -= Time.deltaTime;

            if (timerDisplay != null)
            {
                timerDisplay.text = Mathf.CeilToInt(timeLimit).ToString();
            }

            if (timeLimit <= 0)
            {
                Lose(host.Pick(tooSlowLines));
            }
            else
            {
                SwingMeter();

                if (selectAction.triggered)
                {
                    CheckBow();
                }
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

    void SwingMeter()
    {
        meterValue += meterSpeed * meterDirection * Time.deltaTime;

        if (meterValue >= 1)
        {
            meterValue = 1;
            meterDirection = -1;
        }
        else if (meterValue <= 0)
        {
            meterValue = 0;
            meterDirection = 1;
        }

        MoveMarker();
    }

    void MoveMarker()
    {
        marker.localPosition = new Vector3(0, meterHeight / 2 - meterValue * meterHeight, 0);
    }

    void CheckBow()
    {
        float rightMin;
        float rightMax;
        float wrongMin;
        float wrongMax;
        List<string> wrongZoneLines;

        if (isFormal == true)
        {
            rightMin = formalMin;
            rightMax = formalMax;
            wrongMin = casualMin;
            wrongMax = casualMax;
            wrongZoneLines = tooCasualLines;
        }
        else
        {
            rightMin = casualMin;
            rightMax = casualMax;
            wrongMin = formalMin;
            wrongMax = formalMax;
            wrongZoneLines = tooFormalLines;
        }

        if (meterValue >= rightMin && meterValue <= rightMax)
        {
            pointsEarned = host.SpeedPoints(basePoints, speedBonus, timeLimit, totalTime);

            if (isFormal == true)
            {
                host.Correct(host.Pick(formalWinLines));
            }
            else
            {
                host.Correct(host.Pick(casualWinLines));
            }

            host.ShowPoints(pointsEarned);
            finished = true;
        }
        else if (meterValue >= wrongMin && meterValue <= wrongMax)
        {
            Lose(host.Pick(wrongZoneLines));
        }
        else
        {
            Lose(host.Pick(missLines));
        }
    }

    void Lose(string message)
    {
        host.Wrong(message);
        host.ShowPoints(0);
        finished = true;
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
