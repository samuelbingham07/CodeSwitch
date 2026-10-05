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

    public int pointsPerAnswer = 20;
    public int wrongPenalty = 10;
    public float resultTime = 1.5f;

    public string rightMessage = "RIGHT!";
    public string wrongMessage = "WRONG!";
    public string tooSlowMessage = "TOO SLOW!";

    public TextMeshProUGUI personDisplay;
    public TextMeshProUGUI timerDisplay;
    public TextMeshProUGUI scoreDisplay;
    public TextMeshProUGUI messageDisplay;

    public AudioClip scrollSound;
    public AudioClip selectSound;
    AudioSource gameAudio;

    int currentChoice = 0;
    float blinkTimer;
    bool cursorVisible = true;

    int currentPerson;
    List<int> personPool = new();
    float personTimer;
    int points = 0;

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

        messageDisplay.text = "";
        ShowScore();
        ShowCursor();
        NextPerson();
    }

    void Update()
    {
        if (finished == false)
        {
            personTimer -= Time.deltaTime;

            if (timerDisplay != null)
            {
                timerDisplay.text = Mathf.CeilToInt(personTimer).ToString();
            }

            if (personTimer <= 0)
            {
                Answer(false, tooSlowMessage);
            }
            else
            {
                MoveCursor();
                BlinkCursor();

                if (selectAction.triggered)
                {
                    ShowCursor();
                    gameAudio.PlayOneShot(selectSound);

                    if (currentChoice == correctAnswers[currentPerson])
                    {
                        Answer(true, rightMessage);
                    }
                    else
                    {
                        Answer(false, wrongMessage);
                    }
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
            points += pointsPerAnswer;
        }
        else
        {
            points -= wrongPenalty;

            if (points < 0)
            {
                points = 0;
            }
        }

        messageDisplay.text = message;
        ShowScore();

        if (personPool.Count > 0)
        {
            NextPerson();
        }
        else
        {
            EndGame();
        }
    }

    void EndGame()
    {
        personDisplay.text = "";

        for (int i = 0; i < cursors.Count; i++)
        {
            cursors[i].SetActive(false);
        }

        messageDisplay.text = "+" + points;
        finished = true;
    }

    void ShowScore()
    {
        if (scoreDisplay != null)
        {
            scoreDisplay.text = points.ToString();
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
