using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Level;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Level level;
    public GameOver gameOver;

    public Text remainingText;
    public Text remainingSubText;
    public Text targetText;
    public Text targetSubtext;
    public Text scoreText;
    public Image[] stars;

    private int _starIndex = 0;

    private void Start()
    {
        _starIndex = 0;

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].enabled = i == _starIndex;
        }
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();

        int visibleStar = 0;
        Debug.Log("Score: " + score);
        Debug.Log("Score1Star: " + level.score1Star);
        Debug.Log("Score2Star: " + level.score2Star);
        Debug.Log("Score3Star: " + level.score3Star);
        if (score >= level.score1Star && score < level.score2Star)
        {
            visibleStar = 1;
        }
        else if (score >= level.score2Star && score < level.score3Star)
        {
            visibleStar = 2;
        }
        else if (score >= level.score3Star)
        {
            visibleStar = 3;
        }

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].enabled = (i == visibleStar);
        }

        _starIndex = visibleStar;
    }

    public void SetTarget(int target) => targetText.text = target.ToString();

    public void SetRemaining(int remaining) => remainingText.text = remaining.ToString();

    public void SetRemaining(string remaining) => remainingText.text = remaining;

    public void SetLevelType(LevelType type)
    {
        switch (type)
        {
            case LevelType.MOVES:
                remainingSubText.text = "moves remaining";
                targetSubtext.text = "target score";
                break;
            case LevelType.TIMER:
                remainingSubText.text = "time remaining";
                targetSubtext.text = "target score";
                break;
        }
    }

    public void OnGameWin(int score)
    {
        gameOver.ShowWin(score, _starIndex);
        if (_starIndex > PlayerPrefs.GetInt(SceneManager.GetActiveScene().name, 0))
        {
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, _starIndex);
        }
    }

    public void OnGameLose() => gameOver.ShowLose();

    public void ResetStarIndex()
    {
        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, 0);
    }

}

