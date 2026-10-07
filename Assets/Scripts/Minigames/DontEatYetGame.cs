using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DontEatYetGame : MonoBehaviour
{
    public InputAction selectAction;

    public string familyPrompt = "DINNER WITH OBAACHAN";
    public string friendsPrompt = "DINNER WITH FRIENDS";
    public GameObject signal;
    public GameObject friendsSignal;

    public float waitTimeMin = 1.5f;
    public float waitTimeMax = 4f;
    public float reactWindow = 1.5f;
    public float friendsWindow = 2f;

    public int basePoints = 50;
    public int speedBonus = 50;
    public float resultTime = 1f;

    public List<string> familyWinLines = new()
    {
        "ITADAKIMASU!",
        "PERFECT TIMING!",
        "OBAACHAN SMILES"
    };

    public List<string> friendsWinLines = new()
    {
        "NO NEED TO WAIT!",
        "DIG IN!",
        "FOOD'S HOT, GO!"
    };

    public List<string> tooSoonLines = new()
    {
        "WHY DIDN'T YOU WAIT?",
        "OBAACHAN ISN'T EVEN SEATED!",
        "HANDS OFF!"
    };

    public List<string> familyTooSlowLines = new()
    {
        "THE FOOD GOT COLD...",
        "EVERYONE STARTED WITHOUT YOU",
        "HELLO? ITADAKIMASU!"
    };

    public List<string> friendsTooSlowLines = new()
    {
        "YOUR FRIENDS ATE IT ALL",
        "WHY ARE YOU WAITING?",
        "THIS ISN'T OBAACHAN'S HOUSE!"
    };

    public TextMeshProUGUI promptDisplay;
    public TextMeshProUGUI timerDisplay;
    public Host host;

    bool isFamily;
    bool signalShown = false;
    float waitTimer;
    float windowTimer;
    float timeLeft;

    bool finished = false;
    bool reported = false;
    int pointsEarned = 0;

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
        if (Random.Range(0, 2) == 0)
        {
            isFamily = true;
            promptDisplay.text = familyPrompt;
            windowTimer = reactWindow;
        }
        else
        {
            isFamily = false;
            promptDisplay.text = friendsPrompt;
            windowTimer = friendsWindow;
        }

        waitTimer = Random.Range(waitTimeMin, waitTimeMax);

        if (isFamily == true)
        {
            timeLeft = waitTimer + reactWindow;
        }
        else
        {
            timeLeft = friendsWindow;
        }
        signal.SetActive(false);

        if (friendsSignal != null)
        {
            if (isFamily == true)
            {
                friendsSignal.SetActive(false);
            }
            else
            {
                friendsSignal.SetActive(true);
            }
        }

    }

    void Update()
    {
        if (finished == false)
        {
            timeLeft -= Time.deltaTime;

            if (timerDisplay != null)
            {
                timerDisplay.text = Mathf.CeilToInt(timeLeft).ToString();
            }

            if (isFamily == true)
            {
                FamilyRound();
            }
            else
            {
                FriendsRound();
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

    void FamilyRound()
    {
        if (signalShown == false)
        {
            waitTimer -= Time.deltaTime;

            if (selectAction.triggered)
            {
                Lose(host.Pick(tooSoonLines));
            }
            else if (waitTimer <= 0)
            {
                signalShown = true;
                signal.SetActive(true);
            }
        }
        else
        {
            windowTimer -= Time.deltaTime;

            if (selectAction.triggered)
            {
                Win();
            }
            else if (windowTimer <= 0)
            {
                Lose(host.Pick(familyTooSlowLines));
            }
        }
    }

    void FriendsRound()
    {
        windowTimer -= Time.deltaTime;

        if (selectAction.triggered)
        {
            Win();
        }
        else if (windowTimer <= 0)
        {
            Lose(host.Pick(friendsTooSlowLines));
        }
    }

    void Win()
    {
        if (isFamily == true)
        {
            pointsEarned = host.SpeedPoints(basePoints, speedBonus, windowTimer, reactWindow);
        }
        else
        {
            pointsEarned = host.SpeedPoints(basePoints, speedBonus, windowTimer, friendsWindow);
        }

        if (isFamily == true)
        {
            host.Correct(host.Pick(familyWinLines));
        }
        else
        {
            host.Correct(host.Pick(friendsWinLines));
        }

        host.ShowPoints(pointsEarned);
        finished = true;
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
