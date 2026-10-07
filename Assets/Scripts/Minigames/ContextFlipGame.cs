using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ContextFlipGame : MonoBehaviour
{
    public InputAction leftAction;
    public InputAction rightAction;
    public InputAction selectAction;

    public GameObject leftCursor;
    public GameObject rightCursor;
    public float blinkTime = 0.4f;

    public AudioClip scrollSound;
    AudioSource flipAudio;

    public string japaneseSetting = "YOU'RE AT DINNER WITH OBAACHAN";
    public string americanSetting = "YOU'RE AT DINNER WITH FRIENDS";
    public float flipTimeMin = 3f;
    public float flipTimeMax = 6f;
    public GameObject flipAnimation;

    public float readTime = 1.5f;

    public List<string> japaneseRightLines = new()
    {
        "OBAACHAN NODS!",
        "SO WELL RAISED!",
        "PERFECT MANNERS!",
        "YOU MADE THE FAMILY PROUD"
    };

    public List<string> americanRightLines = new()
    {
        "SMOOTH!",
        "SO CHILL!",
        "CERTIFIED HOMIE",
        "YOU FIT RIGHT IN"
    };

    public List<string> japaneseWrongLines = new()
    {
        "OBAACHAN GASPED!",
        "WHERE ARE YOUR MANNERS?",
        "MOM IS SO EMBARRASSED",
        "WERE YOU RAISED BY WOLVES?"
    };

    public List<string> americanWrongLines = new()
    {
        "WHY SO FORMAL?",
        "BRO, RELAX",
        "THIS ISN'T A TEA CEREMONY",
        "YOUR FRIENDS ARE CONFUSED"
    };

    public List<string> switchCaughtLines = new()
    {
        "SWITCH CAUGHT!",
        "CODE SWITCH MASTER!",
        "SMOOTH SWITCH!"
    };
    public float timeLimit = 20f;

    public List<string> cues = new();
    public List<string> japaneseAnswers = new();
    public List<string> americanAnswers = new();

    public int basePoints = 5;
    public int speedBonus = 10;
    public float fastWindow = 2f;
    public int switchBonus = 15;
    public float flipBonusWindow = 1.5f;

    public TextMeshProUGUI cueDisplay;
    public TextMeshProUGUI leftDisplay;
    public TextMeshProUGUI rightDisplay;
    public TextMeshProUGUI timerDisplay;
    public Host host;
    public float resultTime = 1.5f;

    bool isJapaneseContext;
    bool japaneseOnLeft;
    int currentCue;
    List<int> cuePool = new();

    float flipTimer;
    float timeSinceFlip;
    bool flipCaught = true;

    int points = 0;

    float answerTimer;

    bool reading = false;
    float readTimer;

    bool cursorOnLeft = true;
    float blinkTimer;
    bool cursorVisible = true;

    bool finished = false;
    bool reported = false;

    private void OnEnable()
    {
        leftAction.Enable();
        rightAction.Enable();
        selectAction.Enable();
    }

    private void OnDisable()
    {
        leftAction.Disable();
        rightAction.Disable();
        selectAction.Disable();
    }

    void Start()
    {
        flipAudio = GetComponent<AudioSource>();

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

        ShowTimer();
        NextCue();
        StartReading();
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
        if (reading == true)
        {
            readTimer -= Time.deltaTime;

            if (readTimer <= 0)
            {
                reading = false;
                timeSinceFlip = 0;
                answerTimer = 0;
                ShowCursor();
            }
        }
        else
        {
            timeLimit -= Time.deltaTime;
            ShowTimer();

            if (timeLimit <= 0)
            {
                EndGame();
            }
            else
            {
                flipTimer -= Time.deltaTime;
                timeSinceFlip += Time.deltaTime;
                answerTimer += Time.deltaTime;

                if (flipTimer <= 0)
                {
                    FlipContext();
                }
                else
                {
                    ChooseAnswer();
                }
            }
        }
    }

    void ChooseAnswer()
    {
        if (leftAction.triggered && cursorOnLeft == false)
        {
            cursorOnLeft = true;
            ShowCursor();
            flipAudio.PlayOneShot(scrollSound);
        }
        else if (rightAction.triggered && cursorOnLeft == true)
        {
            cursorOnLeft = false;
            ShowCursor();
            flipAudio.PlayOneShot(scrollSound);
        }

        BlinkCursor();

        if (selectAction.triggered)
        {
            ShowCursor();
            CheckAnswer(cursorOnLeft);
        }
    }

    void ShowTimer()
    {
        if (timerDisplay != null)
        {
            timerDisplay.text = Mathf.CeilToInt(timeLimit).ToString();
        }
    }

    void StartReading()
    {
        reading = true;
        readTimer = readTime;

        leftCursor.SetActive(false);
        rightCursor.SetActive(false);

        ShowCue();
    }

    void ShowCursor()
    {
        if (cursorOnLeft == true)
        {
            leftCursor.SetActive(true);
            rightCursor.SetActive(false);
        }
        else
        {
            leftCursor.SetActive(false);
            rightCursor.SetActive(true);
        }

        cursorVisible = true;
        blinkTimer = blinkTime;
    }

    void BlinkCursor()
    {
        blinkTimer -= Time.deltaTime;

        if (blinkTimer <= 0)
        {
            if (cursorVisible == true)
            {
                cursorVisible = false;
            }
            else
            {
                cursorVisible = true;
            }

            if (cursorOnLeft == true)
            {
                leftCursor.SetActive(cursorVisible);
            }
            else
            {
                rightCursor.SetActive(cursorVisible);
            }

            blinkTimer = blinkTime;
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
        StartReading();

        if (flipAnimation != null)
        {
            Instantiate(flipAnimation, Vector3.zero, Quaternion.identity);
        }
    }

    string CurrentSetting()
    {
        string setting;

        if (isJapaneseContext == true)
        {
            setting = japaneseSetting;
        }
        else
        {
            setting = americanSetting;
        }

        return setting;
    }

    void NextCue()
    {
        int randomIndex = Random.Range(0, cuePool.Count);
        currentCue = cuePool[randomIndex];
        cuePool.Remove(currentCue);

        if (Random.Range(0, 2) == 0)
        {
            japaneseOnLeft = true;
        }
        else
        {
            japaneseOnLeft = false;
        }

        ShowCue();
    }

    void ShowCue()
    {
        answerTimer = 0;
        cueDisplay.text = CurrentSetting() + ": " + cues[currentCue];

        if (japaneseOnLeft == true)
        {
            leftDisplay.text = japaneseAnswers[currentCue];
            rightDisplay.text = americanAnswers[currentCue];
        }
        else
        {
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
            int earned = host.SpeedPoints(basePoints, speedBonus, fastWindow - answerTimer, fastWindow);
            string message;

            if (isJapaneseContext == true)
            {
                message = host.Pick(japaneseRightLines);
            }
            else
            {
                message = host.Pick(americanRightLines);
            }

            if (flipCaught == false && timeSinceFlip <= flipBonusWindow)
            {
                earned += switchBonus;
                message = host.Pick(switchCaughtLines);
            }

            flipCaught = true;
            points += earned;
            host.Correct(message);
            host.ShowPoints(earned);
        }
        else
        {
            if (isJapaneseContext == true)
            {
                host.Wrong(host.Pick(japaneseWrongLines));
            }
            else
            {
                host.Wrong(host.Pick(americanWrongLines));
            }

            host.ShowPoints(0);
        }

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
        leftCursor.SetActive(false);
        rightCursor.SetActive(false);
        host.ShowPoints(points);
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
