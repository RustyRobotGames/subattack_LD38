using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }


    public void HowToPlay()
    {

    }


    public void ExitGame()
    {
        Debug.Log("Exit was clicked");
        Application.Quit();
    }

}
