using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Host : MonoBehaviour
{
    public GameObject idlePose;
    public GameObject correctPose;
    public GameObject wrongPose;

    public GameObject speechBubble;
    public TextMeshProUGUI speechText;
    public TextMeshProUGUI pointsText;

    public AudioClip rightSound;
    public AudioClip wrongSound;
    AudioSource hostAudio;

    void Start()
    {
        hostAudio = GetComponent<AudioSource>();
        correctPose.SetActive(false);
        wrongPose.SetActive(false);
        speechBubble.SetActive(false);

        if (pointsText != null)
        {
            pointsText.text = "";
        }

        if (idlePose != null)
        {
            idlePose.SetActive(true);
        }
    }

    public void Correct(string message)
    {
        correctPose.SetActive(true);
        wrongPose.SetActive(false);
        HideIdle();
        Say(message);
        hostAudio.PlayOneShot(rightSound);
    }

    public void Wrong(string message)
    {
        correctPose.SetActive(false);
        wrongPose.SetActive(true);
        HideIdle();
        Say(message);
        hostAudio.PlayOneShot(wrongSound);
    }

    public void Say(string message)
    {
        speechBubble.SetActive(true);
        speechText.text = message;
    }

    public void ShowPoints(int amount)
    {
        if (pointsText != null)
        {
            pointsText.text = "+" + amount;
        }
    }

    public int SpeedPoints(int basePoints, int speedBonus, float timeLeft, float totalTime)
    {
        int earned = basePoints;

        if (timeLeft > 0 && totalTime > 0)
        {
            earned += Mathf.CeilToInt(speedBonus * timeLeft / totalTime);
        }

        return earned;
    }

    public string Pick(List<string> lines)
    {
        string line = "";

        if (lines.Count > 0)
        {
            line = lines[Random.Range(0, lines.Count)];
        }

        return line;
    }

    void HideIdle()
    {
        if (idlePose != null)
        {
            idlePose.SetActive(false);
        }
    }
}
