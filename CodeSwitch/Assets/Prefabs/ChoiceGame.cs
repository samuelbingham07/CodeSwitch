using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ChoiceGame : MonoBehaviour
{
    public InputAction horizontalAction;
    public InputAction verticalAction;
    public InputAction selectAction;

    public List<GameObject> cursors = new();
    public int correctChoice = 0;
    public int columns = 3;
    public float blinkTime = 0.4f;

    public float timeLimit = 5f;
    public int winPoints = 100;

    public float resultTime = 1f;

    public string winMessage = "NICE!";
    public string loseMessage = "NOT QUITE";

    public TextMeshProUGUI timerDisplay;
    public TextMeshProUGUI messageDisplay;

    public AudioClip scrollSound;
    public AudioClip selectSound;
    AudioSource choiceAudio;

    int currentChoice = 0;
    float blinkTimer;
    bool cursorVisible = true;
    bool finished = false;
    bool reported = false;
    int pointsEarned = 0;

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
        messageDisplay.text = "";
        ShowCursor();
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

    void Choose()
    {
        ShowCursor();
        choiceAudio.PlayOneShot(selectSound);

        if (currentChoice == correctChoice)
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
            pointsEarned = winPoints;
            messageDisplay.text = winMessage;
            finished = true;
        }
    }

    void WrongChoice()
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
