using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class HonorificRushGame : MonoBehaviour
{
    public InputAction horizontalAction;
    public InputAction selectAction;

    public List<GameObject> cursors = new();
    public float blinkTime = 0.4f;

    public List<string> people = new();
    public List<int> correctAnswers = new();
    public float timePerPerson = 3f;

    public string introText = "HOW DO YOU ADDRESS EACH PERSON?";
    public float introTime = 2f;

    public int basePoints = 5;
    public int speedBonus = 15;
    public float timeBetweenPeople = 1f;
    public float resultTime = 1.5f;

    public List<string> rightLines = new()
    {
        "PERFECT!",
        "SO RESPECTFUL!",
        "THEY FELT THAT RESPECT",
        "NAILED IT!"
    };

    public List<string> wrongLines = new()
    {
        "THAT'S AWKWARD...",
        "THEY'LL REMEMBER THAT",
        "UMM, EXCUSE ME?",
        "WHO RAISED YOU?"
    };

    public List<string> tooSlowLines = new()
    {
        "FORGOT HOW TO TALK?",
        "UMMMM...",
        "THEY WALKED AWAY"
    };

    public TextMeshProUGUI personDisplay;
    public TextMeshProUGUI timerDisplay;
    public Host host;

    public AudioClip scrollSound;
    AudioSource gameAudio;

    int currentChoice = 0;
    float blinkTimer;
    bool cursorVisible = true;

    int currentPerson;
    List<int> personPool = new();
    float personTimer;
    int points = 0;

    bool waiting = true;
    float waitTimer;

    bool finished = false;
    bool reported = false;

    private void OnEnable()
    {
        horizontalAction.Enable();
        selectAction.Enable();
    }

    private void OnDisable()
    {
        horizontalAction.Disable();
        selectAction.Disable();
    }

    void Start()
    {
        gameAudio = GetComponent<AudioSource>();

        for (int i = 0; i < people.Count; i++)
        {
            personPool.Add(i);
        }

        waitTimer = introTime;
        personDisplay.text = introText;

        if (timerDisplay != null)
        {
            timerDisplay.text = "";
        }

        for (int i = 0; i < cursors.Count; i++)
        {
            cursors[i].SetActive(false);
        }
    }

    void Update()
    {
        if (finished == false)
        {
            if (waiting == true)
            {
                WaitForNextPerson();
            }
            else
            {
                PlayRound();
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

    void WaitForNextPerson()
    {
        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0)
        {
            waiting = false;
            ShowCursor();
            NextPerson();
        }
    }

    void PlayRound()
    {
        personTimer -= Time.deltaTime;

        if (timerDisplay != null)
        {
            timerDisplay.text = Mathf.CeilToInt(personTimer).ToString();
        }

        if (personTimer <= 0)
        {
            Answer(false, host.Pick(tooSlowLines));
        }
        else
        {
            MoveCursor();
            BlinkCursor();

            if (selectAction.triggered)
            {
                ShowCursor();

                if (currentChoice == correctAnswers[currentPerson])
                {
                    Answer(true, host.Pick(rightLines));
                }
                else
                {
                    Answer(false, host.Pick(wrongLines));
                }
            }
        }
    }

    void NextPerson()
    {
        int randomIndex = Random.Range(0, personPool.Count);
        currentPerson = personPool[randomIndex];
        personPool.Remove(currentPerson);

        personDisplay.text = people[currentPerson];
        personTimer = timePerPerson;
    }

    void Answer(bool correct, string message)
    {
        if (correct == true)
        {
            int earned = host.SpeedPoints(basePoints, speedBonus, personTimer, timePerPerson);
            points += earned;
            host.Correct(message);
            host.ShowPoints(earned);
        }
        else
        {
            host.Wrong(message);
            host.ShowPoints(0);
        }

        if (personPool.Count > 0)
        {
            waiting = true;
            waitTimer = timeBetweenPeople;

            for (int i = 0; i < cursors.Count; i++)
            {
                cursors[i].SetActive(false);
            }
        }
        else
        {
            EndGame();
        }
    }

    void EndGame()
    {
        for (int i = 0; i < cursors.Count; i++)
        {
            cursors[i].SetActive(false);
        }

        host.ShowPoints(points);
        finished = true;
    }

    void MoveCursor()
    {
        int newChoice = currentChoice;

        if (horizontalAction.triggered)
        {
            if (horizontalAction.ReadValue<float>() > 0)
            {
                newChoice = currentChoice + 1;
            }
            else if (horizontalAction.ReadValue<float>() < 0)
            {
                newChoice = currentChoice - 1;
            }
        }

        if (newChoice >= 0 && newChoice < cursors.Count && newChoice != currentChoice)
        {
            currentChoice = newChoice;
            ShowCursor();
            gameAudio.PlayOneShot(scrollSound);
        }
    }

    void ShowCursor()
    {
        for (int i = 0; i < cursors.Count; i++)
        {
            if (i == currentChoice)
            {
                cursors[i].SetActive(true);
            }
            else
            {
                cursors[i].SetActive(false);
            }
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

            cursors[currentChoice].SetActive(cursorVisible);
            blinkTimer = blinkTime;
        }
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
