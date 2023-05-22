using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Image abilityButtonImage;

    [SerializeField] float launchForce = 400f;
    [SerializeField] float maxDragDistance = 3f;
    [SerializeField] GameObject malePlayer;
    [SerializeField] GameObject femalePlayer;

    private SpriteRenderer spriteRenderer;
    private Vector2 spawnPosition;
    public bool abilityUsed = false;

    public SpriteRenderer SpriteRenderer { get => spriteRenderer; set => spriteRenderer = value; }
    public float LaunchForce { get => launchForce; set => launchForce = value; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        spawnPosition = GetComponent<Rigidbody2D>().position;
        GetComponent<Rigidbody2D>().isKinematic = true;
    }

    private void OnMouseDown()
    {
        // to visualize the on-click give the player a red hue
        spriteRenderer.color = Color.red;
    }

    private void OnMouseUp()
    {
        Vector2 currentPosition = GetComponent<Rigidbody2D>().position;
        Vector2 shootDirection = spawnPosition - currentPosition;
        shootDirection.Normalize();

        GetComponent<Rigidbody2D>().isKinematic = false; //needed to be able to add force
        GetComponent<Rigidbody2D>().AddForce(shootDirection * launchForce);

        spriteRenderer.color = Color.white;
    }

    private void OnMouseDrag()
    {
        // current mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 desiredPosition = mousePosition;

        // limit left drag radius from spawn postition
        float distance = Vector2.Distance(desiredPosition, spawnPosition);
        if (distance > maxDragDistance)
        {
            Vector2 dragDirection = desiredPosition - spawnPosition;
            dragDirection.Normalize();
            desiredPosition = spawnPosition + (dragDirection * maxDragDistance);
        }

        // only allow left drag
        if (desiredPosition.x > spawnPosition.x)
        {
            desiredPosition.x = spawnPosition.x;
        }

        GetComponent<Rigidbody2D>().position = desiredPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(ResetAfterDelay());
    }

    private IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(2);

        // go back to spawn position
        GetComponent<Rigidbody2D>().position = spawnPosition;
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Rigidbody2D>().velocity = Vector3.zero; // needed to stop the movement
    }
}
