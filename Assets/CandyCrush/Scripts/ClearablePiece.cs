using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearablePiece : MonoBehaviour
{
    public AnimationClip clearAnimation;
    protected GamePiece piece;
    private bool isBeingCleared = false;

    public bool IsBeingCleared { get => isBeingCleared; }

    private void Awake()
    {
        piece = GetComponent<GamePiece>();
    }

    public virtual void Clear()
    {
        piece.BoardRef.level.OnPieceCleared(piece); //update score

        isBeingCleared = true;
        StartCoroutine(AnimateClearing());
    }

    private IEnumerator AnimateClearing()
    {
        Animator animator = GetComponent<Animator>();

        if(animator)
        {
            animator.Play(clearAnimation.name);
            yield return new WaitForSeconds(clearAnimation.length);

            Debug.Log("destroy piece");
            Destroy(gameObject);
        }
    }
}
