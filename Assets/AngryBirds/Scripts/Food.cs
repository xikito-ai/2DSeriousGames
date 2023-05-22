using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    [SerializeField] Sprite consumedSprite;

    public bool isGood;
    private bool foodCollected;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (CollisionWithFood(collision))
        {
            StartCoroutine(FoodConsumed());
        }
    }

    private bool CollisionWithFood(Collision2D collision)
    {
        // only allow collecting food once
        if (foodCollected)
        {
            return false;
        }

        Player player = collision.gameObject.GetComponent<Player>();

        // collided with player -> food item collected -> disappears
        // collided with boxes (first contact (contacts[0]) comes from slanted above left or lower) -> food item collected -> disappears
        if (player != null || (collision.contacts[0].normal.y < -0.5))
        {
            return true;
        }

        return false;
    }

    private IEnumerator FoodConsumed()
    {
        foodCollected = true;
        GetComponent<SpriteRenderer>().sprite = consumedSprite;
        GetComponentInChildren<ParticleSystem>().Play();
        yield return new WaitForSeconds(1);

        gameObject.SetActive(false);
    }

}
