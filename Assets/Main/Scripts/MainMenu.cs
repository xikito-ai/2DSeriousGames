using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public GameObject liverGameSprite;
    public GameObject diabetesGameSpirte;
    public GameObject liverGameMain;
    public GameObject diabetesGameMain;

    private int sceneToLoad;
    [SerializeField] private int liverSceneID;
    [SerializeField] private int diabetesSceneID; //make sure to get the right sceneID after combining both games

    private void Update()
    {
        if(sceneToLoad == liverSceneID)
        {
            liverGameMain.SetActive(true);
            diabetesGameMain.SetActive(false);
        }
        else if(sceneToLoad == diabetesSceneID)
        {
            diabetesGameMain.SetActive(true);
            liverGameMain.SetActive(false);
        }
        else //set default to liver
        {
            liverGameMain.SetActive(true);
            diabetesGameMain.SetActive(false);
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + sceneToLoad);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        //master mixer has exposed parameter called "volume"
        audioMixer.SetFloat("volume", volume);
    }

    public void SetSceneIDToSelectedGame(int choice)
    {
        if(choice == 0) //chose liver
        {
            liverGameSprite.SetActive(true);
            diabetesGameSpirte.SetActive(false);
            sceneToLoad = liverSceneID;
        }
        else if(choice == 1) //chose diabetes
        {
            diabetesGameSpirte.SetActive(true);
            liverGameSprite.SetActive(false);
            sceneToLoad = diabetesSceneID;
        }
        else //default set to liver
        {
            diabetesGameSpirte.SetActive(true);
            liverGameSprite.SetActive(false);
            sceneToLoad = liverSceneID;
        }
    }
}
