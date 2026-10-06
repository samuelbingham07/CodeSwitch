using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class StartMenu : MonoBehaviour
{
    public InputAction startAction;
    public TextMeshProUGUI highScoreDisplay;

    public GameObject startText;
    public float blinkTime = 0.5f;

    float blinkTimer;
    bool textVisible = true;
    bool started = false;

    private void OnEnable()
    {
        startAction.Enable();
    }

    private void OnDisable()
    {
        startAction.Disable();
    }

    void Start()
    {
        highScoreDisplay.text = GameManager.instance.highScore.ToString();
        blinkTimer = blinkTime;
    }

    void Update()
    {
        BlinkText();

        if (startAction.triggered)
        {
            PlayGame();
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

            startText.SetActive(textVisible);
            blinkTimer = blinkTime;
        }
    }

    void PlayGame()
    {
        if (started == false)
        {
            started = true;
            GameManager.instance.StartRound();
        }
    }
}
