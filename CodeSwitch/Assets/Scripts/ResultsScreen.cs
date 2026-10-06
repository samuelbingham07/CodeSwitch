using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultsScreen : MonoBehaviour
{
    public InputAction backAction;
    public TextMeshProUGUI scoreDisplay;
    public TextMeshProUGUI highScoreDisplay;

    public float returnTimer = 8f;
    public float inputDelay = 1f;

    public GameObject backText;
    public float blinkTime = 0.5f;

    float blinkTimer;
    bool textVisible = true;

    private void OnEnable()
    {
        backAction.Enable();
    }

    private void OnDisable()
    {
        backAction.Disable();
    }

    void Start()
    {
        scoreDisplay.text = "SCORE: " + GameManager.instance.score;
        highScoreDisplay.text = "HIGH SCORE: " + GameManager.instance.highScore;
        blinkTimer = blinkTime;
    }

    void Update()
    {
        returnTimer -= Time.deltaTime;
        inputDelay -= Time.deltaTime;

        BlinkText();

        if (returnTimer <= 0)
        {
            BackToStart();
        }
        else if (backAction.triggered && inputDelay <= 0)
        {
            BackToStart();
        }
    }

    void BlinkText()
    {
        blinkTimer -= Time.deltaTime;

        if (blinkTimer <= 0)
        {
            if (textVisible == true)
            {
                textVisible = false;
            }
            else
            {
                textVisible = true;
            }

            backText.SetActive(textVisible);
            blinkTimer = blinkTime;
        }
    }

    void BackToStart()
    {
        SceneManager.LoadScene(0);
    }
}
