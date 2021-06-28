using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Script sencillo usado para cargar la escena del juego por el nivel indicado
/// </summary>
public class NewGame : MonoBehaviour
{
    public void StartNewLevel()
    {
        PlayerPrefs.SetInt("Level", 1);
        PlayerPrefs.SetInt("Health", 100);
        SceneManager.LoadScene("GameScene");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene("CreditsScene");
    }

}