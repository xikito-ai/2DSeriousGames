using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public Board board;
    public HUD hud;
    protected LevelType type;
    protected int currentScore = 0;

    public int score1Star;
    public int score2Star;
    public int score3Star;

    private bool _didWin;

    public LevelType Type { get { return type; } }

    public enum LevelType
    {
        TIMER, //limited time
        MOVES //limited #moves
    }

    private void Start()
    {
        ResetScore();
        hud.SetScore(currentScore);
    }

    public virtual void GameWin()
    {
        Debug.Log("You win!");
        board.GameOver();

        _didWin = true;
        StartCoroutine(WaitForGridFill());
    }

    public virtual void GameLose()
    {
        Debug.Log("You lose!");
        board.GameOver();

        _didWin = false;
        StartCoroutine(WaitForGridFill());
    }

    public virtual void OnMove()
    {
        Debug.Log("You moved");
    }

    public void OnPieceCleared(GamePiece piece)
    {
        //update score
        currentScore += piece.FoodComponent.scorePoints;
        Debug.Log("current score: " + currentScore);
        hud.SetScore(currentScore);
    }

    protected virtual IEnumerator WaitForGridFill()
    {
        while (board.IsFilling)
        {
            yield return null;
        }

        if (_didWin)
        {
            hud.OnGameWin(currentScore);
        }
        else
        {
            hud.OnGameLose();
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        hud.SetScore(0);
    }
}
