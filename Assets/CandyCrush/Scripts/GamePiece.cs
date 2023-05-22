using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    private int x;
    private int y;

    public int X
    {
        get { return x; }
        set
        {
            if (IsMovable())
            {
                x = value;
            }
        }
    }
    public int Y
    {
        get { return y; }
        set
        {
            if (IsMovable())
            {
                // only allow movement if it is an movable piece
                y = value;
            }
        }
    }

    private Board.PieceType type;
    public Board.PieceType Type
    {
        get { return type; }
    }

    private Board boardRef;
    public Board BoardRef
    {
        get
        {
            return boardRef;
        }
    }

    private MovablePiece movableComponent;
    public MovablePiece MovableComponent
    {
        get { return movableComponent; }
    }

    private FoodPiece foodComponent;
    public FoodPiece FoodComponent
    {
        get { return foodComponent; }
    }

    private ClearablePiece clearableComponent;
    public ClearablePiece ClearableComponent
    {
        get { return clearableComponent; }
    }

    private void Awake()
    {
        movableComponent = GetComponent<MovablePiece>();
        foodComponent = GetComponent<FoodPiece>();
        clearableComponent = GetComponent<ClearablePiece>();
        boardRef = FindObjectOfType<Board>();
    }

    private void OnMouseEnter()
    {
        Debug.Log("mouse enter");
        boardRef.SwapIn(this);
    }

    private void OnMouseDown()
    {
        Debug.Log("mouse down");
        boardRef.SwapOut(this);
    }

    private void OnMouseUp()
    {
        Debug.Log("mouse up");
        boardRef.ReleasePiece();
    }

    public void Init(int init_x, int init_y, Board init_Board, Board.PieceType init_type)
    {
        x = init_x;
        y = init_y;
        boardRef = init_Board;
        type = init_type;
    }

    public bool IsMovable()
    {
        return movableComponent != null;
    }

    public bool IsFood()
    {
        return foodComponent != null;
    }

    public bool IsClearable()
    {
        return clearableComponent != null;
    }
}
