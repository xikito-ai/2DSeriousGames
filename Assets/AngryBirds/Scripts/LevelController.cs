using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public TMP_Text levelText;
    private string currentLevelName;
    private string nextLevelName;

    private Food[] foodItems;
    private List<Food> badFoods = new List<Food>();
    private List<Food> goodFoods = new List<Food>();

    //private Background background;

    private void Awake()
    {
        currentLevelName = SceneManager.GetActiveScene().name;
        foodItems = FindObjectsOfType<Food>();
        SplitFoodItemsIntoType(foodItems, goodFoods, badFoods);
        levelText.text = currentLevelName;
    }

    // Update is called once per frame
    void Update()
    {
        // check if there are existing badFoods at the start of the level
        // CASE 1: no starting badFoods
        if (badFoods.Count == 0)
        {
            if (GoodFoodsAllConsumed())
            {
                GoToNextLevel();
            }
        }
        // CASE 2: there are starting badFoods
        else
        {
            // GAME OVER: restart level if an BadFoods was hit (GoodFoods hits afterwards do not matter)
            if (BadFoodsHit() && badFoods.Count != 0)
            {
                StartCoroutine(DelayRestart());
            }
            else if (GoodFoodsAllConsumed())
            {
                GoToNextLevel();
            }
        }
    }

    private IEnumerator DelayRestart()
    {
        yield return new WaitForSeconds(1);
        RestartSameLevel();
    }

    public void RestartSameLevel()
    {
        Debug.Log("Retry " + currentLevelName);

        SceneManager.LoadScene(currentLevelName);
    }

    private bool BadFoodsHit()
    {
        // check if an BadFoods is hit
        foreach (Food badFood in badFoods)
        {
            if (badFood.gameObject.activeSelf)
            {
                return false;
            }
        }
        Debug.Log("Oh, no! BadFoods Collected! :(");
        return true;
    }

    private void GoToNextLevel()
    {
        // levelName is to be named in the format "Level [int]" e.g. Level 1
        int currentLevel = (int) Char.GetNumericValue(currentLevelName[6]);
        int nextLevel = (int)currentLevel + 1;

        // check if scene for nextLevel belongs to this mini-game (levels 1-3 in this game), if not return to first level (Level 1)
        if (nextLevel >= 4)
        {
            nextLevel = 1;
        }
        nextLevelName = currentLevelName.Replace(currentLevel.ToString(), nextLevel.ToString());

        Debug.Log($"Go from {currentLevelName} to {nextLevelName}");
        SceneManager.LoadScene(nextLevelName);
    }

    private bool GoodFoodsAllConsumed()
    {
        // check if all GoodFoods are collected
        foreach (Food goodFood in goodFoods)
        {
            if (goodFood.gameObject.activeSelf)
            {
                return false;
            }
        }

        Debug.Log("All GoodFoods Collected! :)");
        return true;
    }

    private void SplitFoodItemsIntoType(Food[] foodItems, List<Food> goodFoods, List<Food> badFoods)
    {
        foreach(Food x in foodItems)
        {
            // append food item into good food list
            if (x.isGood)
            {
                goodFoods.Add(x);
                Debug.Log("counting good foods: " + goodFoods.Count);
            }
            else
            {
                // append food item into bad food list
                badFoods.Add(x);
                Debug.Log("counting bad foods: " + badFoods.Count);
            }
        }
    }
}
