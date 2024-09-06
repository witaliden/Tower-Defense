using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayAgain : MonoBehaviour
{
    public void playAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void newGame()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void nextScene()
    {
        var currentScene = SceneManager.GetActiveScene().name;
        switch (currentScene)
        {
            case "Level 1":
                SceneManager.LoadScene("Level 2");
                break;
            case "Level 2":
                SceneManager.LoadScene("Level 3");
                break;
            default:
                SceneManager.LoadScene("Level 3");
                break;
        }
    }

    public void prevScene()
    {
        var currentScene = SceneManager.GetActiveScene().name;

        switch (currentScene)
        {
            case "Level 3":
                SceneManager.LoadScene("Level 2");
                break;
            case "Level 2":
                SceneManager.LoadScene("Level 1");
                break;
            default:
                SceneManager.LoadScene("Level 1");
                break;
        }
    }
}
