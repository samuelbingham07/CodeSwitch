using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultsScreen : MonoBehaviour
{
    public TextMeshProUGUI scoreDisplay;
    public TextMeshProUGUI highScoreDisplay;

    public float returnTimer = 8f;

    void Start()
    {
        scoreDisplay.text = "SCORE: " + GameManager.instance.score;
        highScoreDisplay.text = "HIGH SCORE: " + GameManager.instance.highScore;
    }

    void Update()
    {
        returnTimer -= Time.deltaTime;

        if (returnTimer <= 0)
        {
            BackToStart();
        }
    }

    public void BackToStart()
    {
        SceneManager.LoadScene(0);
    }
}
