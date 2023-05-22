using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePiece : MonoBehaviour
{
    private GamePiece piece;
    private IEnumerator moveCoroutine;

    // get piece asap to avoid null ref exceptions
    private void Awake()
    {
        piece = GetComponent<GamePiece>();
    }

    public void Move(int newX, int newY, float time)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = MoveCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutine);
    }

    private IEnumerator MoveCoroutine(int newX, int newY, float time)
    {
        piece.X = newX;
        piece.Y = newY;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = piece.BoardRef.GetWorldPosition(newX, newY);

        // movement takes exactly Time.deltaTime for smoother animation
        for (float t = 0; t <= 1 * time; t += Time.deltaTime)
        {
            piece.transform.position = Vector3.Lerp(startPosition, endPosition, t / time);
            yield return 0; // wait for 1 frame
        }

        piece.transform.position = endPosition;
    }
}
