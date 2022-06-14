using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartRedirect : MonoBehaviour
{
    [SerializeField]
    private string gameSceneName;
    [SerializeField]
    private string tutorialSceneName;

    void Start()
    {
        if (PlayerPrefs.HasKey("TutorialPlayed"))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            SceneManager.LoadScene(tutorialSceneName);
        }
    }

    [ContextMenu("Delete 'TutorialPlayed' key")]
    public void DeleteTutorialPlayedKey()
    {
        PlayerPrefs.DeleteKey("TutorialPlayed");
    }
}
