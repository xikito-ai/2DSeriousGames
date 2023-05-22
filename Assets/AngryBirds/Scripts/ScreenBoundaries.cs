using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScreenBoundaries : MonoBehaviour
{
    private Vector2 screenBounds;
    private float objectHeight;
    private float objectWidth;

    private void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        objectHeight = transform.GetComponent<SpriteRenderer>().bounds.size.y / 2;
        objectWidth = transform.GetComponent<SpriteRenderer>().bounds.size.x / 2;
    }

    void Update()
    {
        Vector3 viewPos = transform.position;

        viewPos.x = Mathf.Clamp(viewPos.x, screenBounds.x, (screenBounds.x * -1));
        viewPos.y = Mathf.Clamp(viewPos.y, screenBounds.y, (screenBounds.y * -1));
        transform.position = viewPos;

    }
}
