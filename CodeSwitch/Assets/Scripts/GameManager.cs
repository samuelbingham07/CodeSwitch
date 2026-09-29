using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int resultsSceneIndex = 1;
    public List<int> minigameScenes = new();
    public int gamesPerRound = 3;

    public int score;
    public int highScore;

    List<int> roundScenes = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void StartRound()
    {
        score = 0;

        List<int> pool = new();
        for (int i = 0; i < minigameScenes.Count; i++)
        {
            pool.Add(minigameScenes[i]);
        }

        roundScenes = new();
        for (int i = 0; i < gamesPerRound; i++)
        {
            if (pool.Count > 0)
            {
                int randomIndex = Random.Range(0, pool.Count);
                roundScenes.Add(pool[randomIndex]);
                pool.Remove(pool[randomIndex]);
            }
        }

        LoadNextGame();
    }

    public void MinigameFinished(int points)
    {
        score += points;
        LoadNextGame();
    }

    void LoadNextGame()
    {
        if (roundScenes.Count > 0)
        {
            int nextScene = roundScenes[0];
            roundScenes.Remove(nextScene);
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            EndRound();
        }
    }

    void EndRound()
    {
        HighScoreCheck(score);
        SceneManager.LoadScene(resultsSceneIndex);
    }

    public void HighScoreCheck(int newScore)
    {
        if (newScore > highScore)
        {
            highScore = newScore;
        }
    }
}
