using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainGameManager : MonoBehaviour
{
    //public Text scoreText;
    //public Text cholesterolText;
    //public Text dailyFatText;
    //public Text highScoreText;

    //settings menu
    public TMP_Text heightText;
    public TMP_Text weightText;
    public TMP_Text ageText;
    public TMP_Text dailyCalories;

    private MainPlayer player;

    private int highScore;
    private int score;
    private int cholesterol;
    private int dailyFat;

    [SerializeField] private AudioSource bgMusic;
    [SerializeField] private AudioSource bombSound;

    private void Awake()
    {
        player = FindObjectOfType<MainPlayer>();
    }

    private void Start()
    {
        //NewGame();
        bgMusic.Play();
    }

    private void Update()
    {
        //update player info in the settings menu
        heightText.text = FindObjectOfType<MainPlayer>().height.ToString();
        weightText.text = FindObjectOfType<MainPlayer>().weight.ToString();
        ageText.text = FindObjectOfType<MainPlayer>().age.ToString();
        dailyCalories.text = FindObjectOfType<MainPlayer>().dailyCalories.ToString();
    }
}
