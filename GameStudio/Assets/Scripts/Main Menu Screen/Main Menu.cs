using System.ComponentModel;
using UnityEngine;

public class MainMenuScene : MonoBehaviour
{
    [SerializeField] private string mSceneToLoad = "GameScene";

    // When the start button is pressed, load the game scene
    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(mSceneToLoad);
    }


    // when quit is pressed , quit the application
    public void QuitGame()
    {
        Application.Quit();
    }

}

