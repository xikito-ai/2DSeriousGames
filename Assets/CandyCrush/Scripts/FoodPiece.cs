using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class FoodPiece : MonoBehaviour
{
    public int scorePoints;
    private FoodType food;
    private SpriteRenderer sprite;
    private Dictionary<FoodType, Sprite> foodSpriteDict;
    public FoodSprite[] foodSprites;

    public enum FoodType
    {
        //fiber rich
        WHOLE_WHEAT_BREAD,
        BEANS,
        BROCCOLI,
        CARROTS,
        BLUEBERRY,
        LEMON,
        ADDED_SUGAR,
        PIE,
        COUNT
    }

    [System.Serializable]
    public struct FoodSprite
    {
        public FoodType food;
        public Sprite sprite;
    }

    public FoodType Food { get { return food; } set => SetFood(value); }

    private void Start()
    {
        SetScorePoints();
    }

    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();

        foodSpriteDict = new Dictionary<FoodType, Sprite>();

        for (int i = 0; i < foodSprites.Length; i++)
        {
            if (!foodSpriteDict.ContainsKey(foodSprites[i].food))
            {
                foodSpriteDict.Add(foodSprites[i].food, foodSprites[i].sprite);
            }
        }

        SetFood(RandomFoodType()); // set initial instantiation to random food type

    }

    public FoodType RandomFoodType()
    {
        int index = UnityEngine.Random.Range(0, foodSprites.Length);
        FoodSprite random = foodSprites[index];
        return random.food;
    }

    public void SetFood(FoodType newFood)
    {
        food = newFood;

        if (foodSpriteDict.ContainsKey(newFood))
        {
            sprite.sprite = foodSpriteDict[newFood];
        }
    }

    public void SetScorePoints()
    {
        switch ((int)food)
        {
            case 6: // added sugar
                scorePoints = -50;
                break;
            case 7: // pie
                scorePoints = -50;
                break;
            default: // fiber rich foods
                scorePoints = 100;
                break;
        }
    }

    public int NumFoods => foodSprites.Length;
}
