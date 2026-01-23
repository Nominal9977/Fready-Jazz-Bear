using UnityEngine;

public class MainMenuScene : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "GameScene";

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}

