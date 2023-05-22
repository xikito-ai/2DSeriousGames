using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayer : MonoBehaviour
{
    public int height;
    public double weight;
    public bool isMale;
    public int age;
    public int activityLevel; //0: no excercise; 1: light exercise(1-3/week); 2: moderate exercise(3-5/week); 3: hard exercise(6-7/week)
    public double dailyCalories;

    public int dailyFat;

    private void Start()
    {
        //load player infor from PlayerPrefs
        height = PlayerPrefs.GetInt("Height", 178);
        weight = PlayerPrefs.GetInt("Weight", 70);
        age = PlayerPrefs.GetInt("Age", 50);
        activityLevel = PlayerPrefs.GetInt("ActivityLevel", 1);
        dailyCalories = PlayerPrefs.GetInt("DailyCalories");

        if(PlayerPrefs.GetString("Gender").Equals("male"))
        {
            isMale = true;
        }

        if(PlayerPrefs.GetString("Gender").Equals("female"))
        {
            isMale = false;
        }

        dailyFat = MaxDailyFat();
    }

    void Update()
    {
        dailyCalories = CalculateCaloricNeeds();
        dailyFat = MaxDailyFat();
    }

    private double CalculateCaloricNeeds()
    {
        //calculate BMR
        double basalMetabolicRate = 0;
        if (isMale)
        {
            basalMetabolicRate = (10 * weight) + (6.25 * height) - (5 * age) + 5;
        }
        else
        {
            //player is woman
            basalMetabolicRate = (10 * weight) + (6.25 * height) - (5 * age) + 161;
        }

        //calculate calories based on activity level
        double calories = 0;
        switch (activityLevel)
        { 
            case 0: //0: no excercise
                calories = basalMetabolicRate * 1.2;
                break;
            case 1: //1: light exercise(1-3/week)
                calories = basalMetabolicRate * 1.375;
                break;
            case 2: //2: moderate exercise(3-5/week)
                calories = basalMetabolicRate * 1.55;
                break;
            case 3: //3: hard exercise(6-7/week)
                calories = basalMetabolicRate * 1.725;
                break;
            default:
                break;
        }

        return calories;
    }

    private int MaxDailyFat()
    {
        //player should consume max of 30% of its daily calories in fat
        int fatCalories = (int)(0.3 * dailyCalories);
        
        return (int)(fatCalories/9); //1g of fat has 9 calories
    }

    public void SetHeight(string _height)
    {
        height = int.Parse(_height);
    }

    public void SetWeight(string _weight)
    {
        weight = int.Parse(_weight);
    }

    public void SetAge(string _age)
    {
        age = int.Parse(_age);
    }

    public void SetActivityLevel(int level)
    {
        activityLevel = level;
    }

    public void SetGenderAsMale(bool _isMale)
    {
        isMale = _isMale;
    }

    public void SafePlayerInfoPermanently()
    {
        PlayerPrefs.SetInt("Height", height);
        PlayerPrefs.SetInt("Weight", (int)weight);
        PlayerPrefs.SetInt("Age", age);
        PlayerPrefs.SetInt("ActivityLevel", activityLevel);
        PlayerPrefs.SetInt("DailyCalories", (int)dailyCalories);

        if(isMale)
        {
            PlayerPrefs.SetString("Gender", "male");
        } else
        {
            PlayerPrefs.SetString("Gender", "female");
        }
    }

    public void ResetHighscore()
    {
        PlayerPrefs.SetInt("Highscore", 0);
    }
}
