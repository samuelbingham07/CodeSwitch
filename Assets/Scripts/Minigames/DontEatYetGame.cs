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

    public int winPoints = 100;
    public float resultTime = 1f;

    public string winMessage = "NICE!";
    public string tooSoonMessage = "TOO SOON!";
    public string tooSlowMessage = "TOO SLOW!";

    public TextMeshProUGUI promptDisplay;
    public TextMeshProUGUI messageDisplay;

    public AudioClip selectSound;
    AudioSource gameAudio;

    bool isFamily;
    bool signalShown = false;
    float waitTimer;
    float windowTimer;

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

        messageDisplay.text = "";
    }

    void Update()
    {
        if (finished == false)
        {
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
                gameAudio.PlayOneShot(selectSound);
                Lose(tooSoonMessage);
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
                gameAudio.PlayOneShot(selectSound);
                Win();
            }
            else if (windowTimer <= 0)
            {
                Lose(tooSlowMessage);
            }
        }
    }

    void FriendsRound()
    {
        windowTimer -= Time.deltaTime;

        if (selectAction.triggered)
        {
            gameAudio.PlayOneShot(selectSound);
            Win();
        }
        else if (windowTimer <= 0)
        {
            Lose(tooSlowMessage);
        }
    }

    void Win()
    {
        pointsEarned = winPoints;
        messageDisplay.text = winMessage;
        finished = true;
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
