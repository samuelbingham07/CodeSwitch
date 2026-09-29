using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ContextFlipGame : MonoBehaviour
{
    public InputAction leftAction;
    public InputAction rightAction;

    public string japaneseContextName = "DINNER WITH OBAACHAN";
    public string americanContextName = "DINNER WITH FRIENDS";
    public float flipTimeMin = 3f;
    public float flipTimeMax = 6f;
    public GameObject flipAnimation;

    public List<string> cues = new();
    public List<string> japaneseAnswers = new();
    public List<string> americanAnswers = new();

    public int pointsPerAnswer = 10;
    public int wrongPenalty = 20;
    public int flipBonus = 50;
    public float flipBonusWindow = 1.5f;

    public TextMeshProUGUI contextDisplay;
    public TextMeshProUGUI cueDisplay;
    public TextMeshProUGUI leftDisplay;
    public TextMeshProUGUI rightDisplay;
    public TextMeshProUGUI scoreDisplay;
    public TextMeshProUGUI messageDisplay;
    public float resultTime = 1.5f;

    bool isJapaneseContext;
    bool japaneseOnLeft;
    int currentCue;
    List<int> cuePool = new();

    float flipTimer;
    float timeSinceFlip;
    bool flipCaught = true;

    int combo = 0;
    int points = 0;

    bool finished = false;
    bool reported = false;

    private void OnEnable()
    {
        leftAction.Enable();
        rightAction.Enable();
    }

    private void OnDisable()
    {
        leftAction.Disable();
        rightAction.Disable();
    }

    void Start()
    {
        if (Random.Range(0, 2) == 0)
        {
            isJapaneseContext = true;
        }
        else
        {
            isJapaneseContext = false;
        }

        flipTimer = Random.Range(flipTimeMin, flipTimeMax);

        for (int i = 0; i < cues.Count; i++)
        {
            cuePool.Add(i);
        }

        messageDisplay.text = "";
        scoreDisplay.text = "0";
        ShowContext();
        NextCue();
    }

    void Update()
    {
        if (finished == false)
        {
            PlayGame();
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

    void PlayGame()
    {
        flipTimer -= Time.deltaTime;
        timeSinceFlip += Time.deltaTime;

        if (flipTimer <= 0)
        {
            FlipContext();
        }

        if (leftAction.triggered)
        {
            CheckAnswer(true);
        }
        else if (rightAction.triggered)
        {
            CheckAnswer(false);
        }
    }

    void FlipContext()
    {
        if (isJapaneseContext == true)
        {
            isJapaneseContext = false;
        }
        else
        {
            isJapaneseContext = true;
        }

        flipTimer = Random.Range(flipTimeMin, flipTimeMax);
        timeSinceFlip = 0;
        flipCaught = false;
        ShowContext();

        if (flipAnimation != null)
        {
            Instantiate(flipAnimation, Vector3.zero, Quaternion.identity);
        }
    }

    void ShowContext()
    {
        if (isJapaneseContext == true)
        {
            contextDisplay.text = japaneseContextName;
        }
        else
        {
            contextDisplay.text = americanContextName;
        }
    }

    void NextCue()
    {
        int randomIndex = Random.Range(0, cuePool.Count);
        currentCue = cuePool[randomIndex];
        cuePool.Remove(currentCue);

        cueDisplay.text = cues[currentCue];

        if (Random.Range(0, 2) == 0)
        {
            japaneseOnLeft = true;
            leftDisplay.text = japaneseAnswers[currentCue];
            rightDisplay.text = americanAnswers[currentCue];
        }
        else
        {
            japaneseOnLeft = false;
            leftDisplay.text = americanAnswers[currentCue];
            rightDisplay.text = japaneseAnswers[currentCue];
        }
    }

    void CheckAnswer(bool pressedLeft)
    {
        bool leftIsCorrect;

        if (isJapaneseContext == japaneseOnLeft)
        {
            leftIsCorrect = true;
        }
        else
        {
            leftIsCorrect = false;
        }

        if (pressedLeft == leftIsCorrect)
        {
            combo++;
            points += pointsPerAnswer * combo;
            messageDisplay.text = "x" + combo;

            if (flipCaught == false && timeSinceFlip <= flipBonusWindow)
            {
                points += flipBonus;
                messageDisplay.text = "SWITCH CAUGHT! +" + flipBonus;
            }
            flipCaught = true;
        }
        else
        {
            combo = 0;
            points -= wrongPenalty;
            messageDisplay.text = "-" + wrongPenalty;

            if (points < 0)
            {
                points = 0;
            }
        }

        scoreDisplay.text = points.ToString();

        if (cuePool.Count > 0)
        {
            NextCue();
        }
        else
        {
            EndGame();
        }
    }

    void EndGame()
    {
        cueDisplay.text = "";
        leftDisplay.text = "";
        rightDisplay.text = "";
        messageDisplay.text = "+" + points;
        finished = true;
    }

    void ReportResult()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.MinigameFinished(points);
        }
        else
        {
            Debug.Log("Minigame over. Points: " + points);
        }
    }
}
