using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    private void Start() {
        // if(AudioManager.instance.isPlaying("Rev")) {
        //     AudioManager.instance.Stop("Rev");
        // }

        // if(AudioManager.instance.isPlaying("Crash")) {
        //     AudioManager.instance.Stop("Crash");
        // }

        Cursor.lockState = CursorLockMode.None;
    }
    public void StartGame()
    {
        SceneManager.LoadScene("CarSelectionScene");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
