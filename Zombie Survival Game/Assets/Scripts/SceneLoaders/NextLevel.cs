using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{

    public void LoadNextLevel()
    {

        SceneManager.LoadScene("GameScene");
    }

    public void SetNextLevelParameters(int level, int health)
    {
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("Health", health);
    }
}
