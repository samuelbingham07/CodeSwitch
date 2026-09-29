using UnityEngine;
using TMPro;

public class StartMenu : MonoBehaviour
{
    public TextMeshProUGUI highScoreDisplay;

    void Start()
    {
        highScoreDisplay.text = GameManager.instance.highScore.ToString();
    }

    public void PlayGame()
    {
        GameManager.instance.StartRound();
    }
}
