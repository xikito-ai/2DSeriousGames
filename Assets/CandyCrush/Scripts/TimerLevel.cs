using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerLevel : Level
{
    public int scoreGoal;
    public int timeInSeconds;
    private float _timer;

    void Start()
    {
        type = LevelType.TIMER;

        currentScore = 0;

        hud.SetLevelType(type);
        hud.SetScore(0);
        hud.SetTarget(scoreGoal);
        hud.SetRemaining($"{timeInSeconds / 60}:{timeInSeconds % 60:00}");
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        hud.SetRemaining(
            $"{(int)Mathf.Max((timeInSeconds - _timer) / 60, 0)}:{(int)Mathf.Max((timeInSeconds - _timer) % 60, 0)}");

        if (timeInSeconds - _timer <= 0)
        {
            if (currentScore >= scoreGoal)
            {
                GameWin();
            }
            else
            {
                GameLose();
            }
        }
    }

}
