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
    public GameObject formalZone;
    public GameObject casualZone;

    public float timeLimit = 5f;
    public int winPoints = 100;
    public float resultTime = 1f;

    public string winMessage = "NICE!";
    public string tooDeepMessage = "TOO DEEP!";
    public string tooShallowMessage = "TOO SHALLOW!";
    public string loseMessage = "NOT QUITE";

    public TextMeshProUGUI promptDisplay;
    public TextMeshProUGUI timerDisplay;
    public TextMeshProUGUI messageDisplay;

    public AudioClip selectSound;
    AudioSource gameAudio;

    bool isFormal;
    float meterValue = 0;
    float meterDirection = 1;

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
        gameAudio = GetComponent<AudioSource>();

        if (Random.Range(0, 2) == 0)
        {
            isFormal = true;
            promptDisplay.text = formalPrompt;
            formalZone.SetActive(true);
            casualZone.SetActive(false);
        }
        else
        {
            isFormal = false;
            promptDisplay.text = casualPrompt;
            formalZone.SetActive(false);
            casualZone.SetActive(true);
        }

        messageDisplay.text = "";
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
                Lose(loseMessage);
            }
            else
            {
                SwingMeter();

                if (selectAction.triggered)
                {
                    gameAudio.PlayOneShot(selectSound);
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
        float zoneMin;
        float zoneMax;

        if (isFormal == true)
        {
            zoneMin = formalMin;
            zoneMax = formalMax;
        }
        else
        {
            zoneMin = casualMin;
            zoneMax = casualMax;
        }

        if (meterValue >= zoneMin && meterValue <= zoneMax)
        {
            pointsEarned = winPoints;
            messageDisplay.text = winMessage;
            finished = true;
        }
        else if (meterValue > zoneMax)
        {
            Lose(tooDeepMessage);
        }
        else
        {
            Lose(tooShallowMessage);
        }
    }

    void Lose(string message)
    {
        messageDisplay.text = message;
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
