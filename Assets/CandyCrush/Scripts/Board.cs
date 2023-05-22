using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

public class Board : MonoBehaviour
{
    public int xDim;
    public int yDim;

    private GamePiece swappedInPiece;
    private GamePiece swappedOutPiece;

    public GameObject backgroundPrefab;
    public PiecePrefab[] piecePrefabs; //array of piecePrefabs shown in the inspector
    public GamePiece[,] piecesOnBoard; //2D array of game pieces on the board
    public Dictionary<PieceType, GameObject> piecePrefabDict;

    private bool gameOver;
    public float fillTime = 2f;

    public Level level;

    public bool IsFilling { get; private set; }
    public GamePiece SwappedOutPiece { get => swappedOutPiece; set => swappedOutPiece = value; }
    public GamePiece SwappedInPiece { get => swappedInPiece; set => swappedInPiece = value; }

    //collect the type of pieces in the Board
    public enum PieceType
    {
        EMPTY,
        NORMAL,
        ROW_CLEAR,
        COLUMN_CLEAR,
        COUNT
    }

    [System.Serializable]
    public struct PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;
    }

    void Start()
    {
        #region(DICTIONARY SETUP)
        piecePrefabDict = new Dictionary<PieceType, GameObject>();

        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            // check for each key if empty because each key can only contain one gameObject as its value
            if (!piecePrefabDict.ContainsKey(piecePrefabs[i].type))
            {
                piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
        }
        #endregion

        // background Board
        for (int x = -2; x < xDim - 2; x++) // board has rect transform posX=2
        {
            for (int y = 0; y < yDim; y++)
            {
                GameObject background = (GameObject)Instantiate(backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);
                background.transform.SetParent(transform, false); // make new bg child of Board object
            }
        }

        // game pieces in the bg Board
        piecesOnBoard = new GamePiece[xDim, yDim];
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                if (piecesOnBoard[x,y] == null)
                {
                    // fill board with emtpy pieces
                    SpawnNewPiece(x, y, PieceType.EMPTY);
                }
            }
        }

        // fill board with normal pieces
        FirstBoardFill();
    }

    public void FirstBoardFill()
    {
        StartCoroutine(FillBoard());
        level.ResetScore(); // don't count initial matches
    }

    private void Update()
    {
        StartCoroutine(FillBoard());
    }

    private IEnumerator FillBoard()
    {
        AllValidMatchesCleared();

        // keep filling until board is filled
        while (EmptyExistsOnBoard())
        {
            yield return new WaitForSeconds(fillTime);
            FillEmptyWithAbove();
        }
    }

    private bool EmptyExistsOnBoard()
    {
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                if (piecesOnBoard[x, y].Type == PieceType.EMPTY) return true;
            }
        }
        return false;
    }

    private void FillEmptyWithAbove()
    {
        #region(HINT: how the board indices work)
        ///y = 0: first row at the very top of the board
        ///y = yDim - 1: most bottom row of the board
        ///x = 0: most left "grid" in a row
        ///x = xDim - 1: most right "grid" in a row
        #endregion

        // use (yDim - 2) because the second most bottom row is the first row that can move downwards
        for (int y = yDim - 2; y >= 0; y--) // row
        {
            for (int x = 0; x < xDim; x++) // column
            {
                GamePiece piece = piecesOnBoard[x, y];

                if (!piece.IsMovable()) continue;

                GamePiece pieceBelow = piecesOnBoard[x, y + 1];

                // as long as pieces are of type empty: fill up with the piece from above
                if (pieceBelow.Type == PieceType.EMPTY)
                {
                    // remove empty game piece
                    Destroy(pieceBelow.gameObject);

                    // move current piece down
                    piece.MovableComponent.Move(x, y + 1, fillTime);
                    piecesOnBoard[x, y + 1] = piece;

                    // refill former space with empty piece type
                    SpawnNewPiece(x, y, PieceType.EMPTY);
                }
                
            }

            // now check the first row (0) and fill from "invisible row above" if space is emtpy
            for (int x = 0; x < xDim; x++)
            {
                GamePiece pieceBelow = piecesOnBoard[x, 0];

                if (pieceBelow.Type != PieceType.EMPTY) continue;

                // remove empty game piece
                Destroy(pieceBelow.gameObject);

                // instantiate a normal game piece in the "invisible row above"
                GameObject newPiece = Instantiate(piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity, this.transform);
                piecesOnBoard[x, 0] = newPiece.GetComponent<GamePiece>();
                piecesOnBoard[x, 0].Init(x, -1, this, PieceType.NORMAL);

                // move normal game piece from above down to refill space in first row
                piecesOnBoard[x, 0].MovableComponent.Move(x, 0, fillTime);
                piecesOnBoard[x, 0].FoodComponent.SetFood(piecesOnBoard[x, 0].FoodComponent.RandomFoodType());
            }
        }
    }

    public Vector2 GetWorldPosition(int x, int y)
    {
        // get the location of the world to center game pieces in the Board
        Vector2 boardCenter = new(xDim / 2, yDim / 2);
        return new Vector2(transform.position.x - boardCenter.x + x, transform.position.y + boardCenter.y - y);
    }

    public GamePiece SpawnNewPiece(int x, int y, PieceType type)
    {
        GameObject newPiece = Instantiate(piecePrefabDict[type], new Vector3(x, y, Vector3.zero.z), Quaternion.identity
            );
        newPiece.transform.parent = transform;

        piecesOnBoard[x, y] = newPiece.GetComponent<GamePiece>();
        piecesOnBoard[x, y].Init(x, y, this, type);

        return piecesOnBoard[x, y];
    }

    private bool IsAdjacent(GamePiece piece1, GamePiece piece2)
    {
        // adjacent, if in the same column AND only 1 row apart OR in the same row AND only 1 column apart
        return (piece1.X == piece2.X && Mathf.Abs(piece1.Y - piece2.Y) == 1) || (piece1.Y == piece2.Y && Mathf.Abs(piece1.X - piece2.X) == 1);
    }

    public void SwapPieces(GamePiece piece1, GamePiece piece2)
    {
        if (gameOver) { Debug.Log("game over"); return; } // disable movement if game is over

        if (piece1.IsMovable() && piece2.IsMovable())
        {
            piecesOnBoard[piece1.X, piece1.Y] = piece2;
            piecesOnBoard[piece2.X, piece2.Y] = piece1;

            // swap only if the swap action would lead to a three match
            if (GetThreeMatches(piece1, piece2.X, piece2.Y) != null || GetThreeMatches(piece2, piece1.X, piece1.Y) != null) 
            {
                // match

                // store position coordinates of one piece to access it after its changed
                int piece1X = piece1.X;
                int piece1Y = piece1.Y;

                piece1.MovableComponent.Move(piece2.X, piece2.Y, fillTime);
                piece2.MovableComponent.Move(piece1X, piece1Y, fillTime);

                AllValidMatchesCleared();

                //Special Piece type pieces get cleared no matter who did the match
                if (piece1.Type == PieceType.ROW_CLEAR || piece1.Type == PieceType.COLUMN_CLEAR)
                {
                    Debug.Log("starting ClearPiece");
                    ClearPiece(piece1.X, piece1.Y);
                }

                if (piece2.Type == PieceType.ROW_CLEAR || piece2.Type == PieceType.COLUMN_CLEAR)
                {
                    ClearPiece(piece2.X, piece2.Y);
                }

                swappedInPiece = null;
                swappedOutPiece = null;

                level.OnMove(); // in case of level type 'moves limit' this here decreases the moveCount
            }
            else 
            {
                // no match

                //do not swap and the positions of the two pieces remain untouched on the board
                piecesOnBoard[piece1.X, piece1.Y] = piece1;
                piecesOnBoard[piece2.X, piece2.Y] = piece2;
            }
        }
    }

    public List<GamePiece> GetThreeMatches(GamePiece piece, int newX, int newY)
    {
        // match only possible if it is food
        if (!piece.IsFood()) return null; 
        
        FoodPiece.FoodType food = piece.FoodComponent.Food;
        List<GamePiece> horizontalPieces = new List<GamePiece>();
        List<GamePiece> verticalPieces = new List<GamePiece>();
        List<GamePiece> matchingThreePieces = new List<GamePiece>();

        #region([1] HORIZONTAL CHECK)
        horizontalPieces = CheckHorizontalMatches(piece, newX, newY);

        Debug.Log($"IN GTM: horizontalPieces match count: {horizontalPieces.Count}");

        // check if is 3 match
        if (horizontalPieces.Count >= 3)
        {
            foreach(GamePiece horizontalPiece in horizontalPieces)
            {
                // add horizontal matches to final matchingThreePiece result list
                matchingThreePieces.Add(horizontalPiece);

                // traverse vertically to see if the horizontal matches also match upper or lower pieces (T or L shaped matches)
                List<GamePiece> verticalMatchFromHorizontal = new List<GamePiece>();
                verticalMatchFromHorizontal = CheckVerticalMatches(horizontalPiece, horizontalPiece.X, horizontalPiece.Y);

                // remove horizontal match so that it is not added to result list again; check if 2 vertical matches appeared that result in a 3 match with the current horizontal piece
                verticalMatchFromHorizontal.Remove(horizontalPiece);
                if (verticalMatchFromHorizontal.Count < 2)
                {
                    verticalMatchFromHorizontal.Clear();
                }
                else
                {
                    foreach (GamePiece match in verticalMatchFromHorizontal)
                    {
                        matchingThreePieces.Add(match);
                    }
                    break;
                }
            }
        }

        // return all matches if at least one 3 match exists
        if (matchingThreePieces.Count >= 3)
        {
            return matchingThreePieces;
        }
        #endregion

        horizontalPieces.Clear();
        verticalPieces.Clear();

        // no horizontal 3 matches found -> now check vertically
        #region([2] VERTICAL CHECK)
        verticalPieces = CheckVerticalMatches(piece, newX, newY);

        // check if is 3 match
        if (verticalPieces.Count >= 3)
        {
            foreach(GamePiece verticalPiece in verticalPieces)
            {
                matchingThreePieces.Add(verticalPiece);

                // traverse horizontally to see if the vertical matches also match left or right pieces(T or L shaped matches)
                List<GamePiece> horizontalMatchFromVertical = new List<GamePiece>();
                horizontalMatchFromVertical = CheckHorizontalMatches(verticalPiece, verticalPiece.X, verticalPiece.Y);

                // remove horizontal match so that it is not added to result list again
                horizontalMatchFromVertical.Remove(verticalPiece);
                if (horizontalMatchFromVertical.Count < 2)
                {
                    horizontalMatchFromVertical.Clear();
                }
                else
                {
                    foreach (GamePiece match in horizontalMatchFromVertical)
                    {
                        matchingThreePieces.Add(match);
                    }
                    break;
                }
            }
        }

        if (matchingThreePieces.Count >= 3)
        {
            return matchingThreePieces;
        }
        #endregion
  
        // no 3 matches found
        return null;

    }

    private List<GamePiece> CheckHorizontalMatches(GamePiece piece, int newX, int newY)
    {
        FoodPiece.FoodType food = piece.FoodComponent.Food;
        List<GamePiece> matches = new List<GamePiece>{piece};

        for (int dir = 0; dir <= 1; dir++)
        {
            for (int xOffset = 1; xOffset < xDim; xOffset++)
            {
                int x;

                if (dir == 0) // we go left
                {
                    x = newX - xOffset;
                }
                else // we go right
                {
                    x = newX + xOffset;
                }

                // out-of-bounds
                if (x < 0 || x >= xDim) { break; }

                // check if piece is same food item
                if (piecesOnBoard[x, newY].IsFood() && piecesOnBoard[x, newY].FoodComponent.Food == food)
                {
                    matches.Add(piecesOnBoard[x, newY]);
                }
                else
                {
                    break;
                }
            }
        }
    
        return matches;
    }

    private List<GamePiece> CheckVerticalMatches(GamePiece piece, int newX, int newY)
    {
        FoodPiece.FoodType food = piece.FoodComponent.Food;
        List<GamePiece> matches = new List<GamePiece> { piece };

        for (int dir = 0; dir <= 1; dir++)
        {
            for (int yOffset = 1; yOffset < yDim; yOffset++)
            {
                int y;

                if (dir == 0) // we go up
                {
                    y = newY - yOffset;
                }
                else // we go down
                {
                    y = newY + yOffset;
                }

                // out-of-bounds
                if (y < 0 || y >= yDim) { break; }

                // check if is piece is same food item
                if (piecesOnBoard[newX, y].IsFood() && piecesOnBoard[newX, y].FoodComponent.Food == food)
                {
                    matches.Add(piecesOnBoard[newX, y]);
                }
                else
                {
                    break;
                }
            }
        }

        return matches;
    }

    public void SwapOut(GamePiece piece)
    {
        // piece that gets moved out
        swappedOutPiece = piece;
    }

    public void SwapIn(GamePiece piece)
    {
        // piece that gets moved in
        swappedInPiece = piece;
    }

    public void SwapAdjacentPieces()
    {
        if (IsAdjacent(swappedInPiece, swappedOutPiece))
        {
            //initiating actual swap process
            SwapPieces(swappedInPiece, swappedOutPiece);
        }
    }

    // returns true, if all matches got cleared
    public bool AllValidMatchesCleared()
    {
        bool needsRefill = false;

        for (int y = 0; y < yDim; y++)
        {
            for (int x = 0; x < xDim; x++)
            {
                if (piecesOnBoard[x, y].IsClearable())
                {
                    List<GamePiece> matches = GetThreeMatches(piecesOnBoard[x, y], x, y);

                    if (matches != null) 
                    {
                        // matches found
                  
                        // Clear all match pieces
                        foreach (GamePiece match in matches)
                        {
                            if (!ClearPiece(match.X, match.Y)) continue;

                            needsRefill = true;
                        }
                    }
                }
            }
        }
        return needsRefill;
    }

    public bool ClearPiece(int x, int y)
    {
        if (piecesOnBoard[x, y].IsClearable() && !piecesOnBoard[x, y].ClearableComponent.IsBeingCleared)
        {
            // piece is clearable and has not been cleared yet
            piecesOnBoard[x, y].ClearableComponent.Clear();
            SpawnNewPiece(x, y, PieceType.EMPTY);

            return true;
        }

        return false;
    }

    public void ClearRow(int row)
    {
        for (int x =0; x < xDim; x++)
        {
            ClearPiece(x, row);
        }

    }

    public void ClearColumn(int column)
    {
        for (int y = 0; y < yDim; y++)
        {
            ClearPiece(y, column);
        }

    }

    public void GameOver()
    {
        gameOver = true;
    }

    public void ReleasePiece()
    {
        if (IsAdjacent(swappedOutPiece, swappedInPiece))
        {
            SwapPieces(swappedOutPiece, swappedInPiece);
        }
    }
}
