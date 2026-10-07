using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ChoiceGame : MonoBehaviour
{
    public InputAction horizontalAction;
    public InputAction verticalAction;
    public InputAction selectAction;

    public List<GameObject> options = new();
    public List<GameObject> cursors = new();
    public int correctChoice = 0;
    public int columns = 3;
    public float blinkTime = 0.4f;

    public float timeLimit = 5f;
    public int basePoints = 50;
    public int speedBonus = 50;

    public float resultTime = 1f;

    public List<string> winLines = new()
    {
        "OBAACHAN APPROVES!",
        "SUCH GOOD MANNERS!",
        "YOUR MOM IS SO PROUD",
        "THE AUNTIES WILL BRAG ABOUT YOU",
        "MODEL GRANDCHILD!"
    };

    public List<string> wrongLines = new()
    {
        "THE AUNTIES ARE WHISPERING...",
        "OBAACHAN SAW THAT.",
        "MOM IS GIVING YOU THE LOOK",
        "THAT'S GOING IN THE FAMILY GROUP CHAT",
        "BACHI GA ATARU!"
    };

    public List<string> tooSlowLines = new()
    {
        "THINKING TOO HARD?",
        "THEY'RE STILL WAITING...",
        "SAY SOMETHING!"
    };

    public TextMeshProUGUI timerDisplay;
    public Host host;

    public AudioClip scrollSound;
    AudioSource choiceAudio;

    int currentChoice = 0;
    List<int> spotOrder = new();
    float blinkTimer;
    bool cursorVisible = true;
    bool finished = false;
    bool reported = false;
    int pointsEarned = 0;
    float totalTime;

    private void OnEnable()
    {
        horizontalAction.Enable();
        verticalAction.Enable();
        selectAction.Enable();
    }

    private void OnDisable()
    {
        horizontalAction.Disable();
        verticalAction.Disable();
        selectAction.Disable();
    }

    void Start()
    {
        choiceAudio = GetComponent<AudioSource>();
        totalTime = timeLimit;
        ShuffleOptions();
        ShowCursor();
    }

    void ShuffleOptions()
    {
        if (options.Count == cursors.Count)
        {
            List<Vector3> spots = new();
            List<int> pool = new();

            for (int i = 0; i < options.Count; i++)
            {
                spots.Add(options[i].transform.localPosition);
                pool.Add(i);
            }

            for (int spot = 0; spot < spots.Count; spot++)
            {
                int randomIndex = Random.Range(0, pool.Count);
                int option = pool[randomIndex];
                pool.Remove(option);

                spotOrder.Add(option);
                options[option].transform.localPosition = spots[spot];
            }
        }
        else
        {
            for (int i = 0; i < cursors.Count; i++)
            {
                spotOrder.Add(i);
            }
        }
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
                Lose(host.Pick(tooSlowLines));
            }
            else
            {
                MoveCursor();
                BlinkCursor();

                if (selectAction.triggered)
                {
                    Choose();
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

        if (verticalAction.triggered)
        {
            if (verticalAction.ReadValue<float>() > 0)
            {
                newChoice = currentChoice - columns;
            }
            else if (verticalAction.ReadValue<float>() < 0)
            {
                newChoice = currentChoice + columns;
            }
        }

        if (newChoice >= 0 && newChoice < cursors.Count && newChoice != currentChoice)
        {
            currentChoice = newChoice;
            ShowCursor();
            choiceAudio.PlayOneShot(scrollSound);
        }
    }

    void ShowCursor()
    {
        for (int i = 0; i < cursors.Count; i++)
        {
            if (i == spotOrder[currentChoice])
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

            cursors[spotOrder[currentChoice]].SetActive(cursorVisible);
            blinkTimer = blinkTime;
        }
    }

    void Choose()
    {
        ShowCursor();

        if (spotOrder[currentChoice] == correctChoice)
        {
            CorrectChoice();
        }
        else
        {
            WrongChoice();
        }
    }

    void CorrectChoice()
    {
        if (finished == false)
        {
            pointsEarned = host.SpeedPoints(basePoints, speedBonus, timeLimit, totalTime);
            host.Correct(host.Pick(winLines));
            host.ShowPoints(pointsEarned);
            finished = true;
        }
    }

    void WrongChoice()
    {
        Lose(host.Pick(wrongLines));
    }

    void Lose(string message)
    {
        if (finished == false)
        {
            host.Wrong(message);
            host.ShowPoints(0);
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
