using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovesLevel : Level
{
    public int scoreGoal;
    public int movesLimit;
    private int movesUsed = 0;

    void Start()
    {
        type = LevelType.MOVES;

        currentScore = 0;
        hud.SetLevelType(type);
        hud.SetScore(currentScore);
        hud.SetTarget(scoreGoal);
        hud.SetRemaining(movesLimit);
    }

    public override void OnMove()
    {
        movesUsed++;

        hud.SetRemaining(movesLimit - movesUsed);

        if (movesLimit - movesUsed != 0) return;

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
