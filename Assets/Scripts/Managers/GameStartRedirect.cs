using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MPack;

public class GameStartRedirect : MonoBehaviour
{
    [SerializeField]
    private LanguageData english;
    [SerializeField]
    private LanguageData chinese;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip submitSound;

    [Header("Change scene")]
    [SerializeField]
    private float changeSceneDelay;
    [SerializeField]
    private string gameSceneName;
    [SerializeField]
    private string tutorialSceneName;

    void Awake()
    {
        if (PlayerPrefs.HasKey("TutorialPlayed"))
            SceneManager.LoadScene(gameSceneName);

    }

    public void SwitchToChinese()
    {
        LanguageMgr.AssignLanguageData(chinese);
    }

    public void SwitchToEnglish()
    {
        LanguageMgr.AssignLanguageData(english);
    }

    public void ChoseChinese()
    {
        PlayerPrefs.SetInt("Language", chinese.ID);
        audioSource.PlayOneShot(submitSound);
        StartCoroutine(LoadTutorialScene());
    }

    public void ChoseEnglish()
    {
        PlayerPrefs.SetInt("Language", english.ID);
        audioSource.PlayOneShot(submitSound);
        StartCoroutine(LoadTutorialScene());
    }


    IEnumerator LoadTutorialScene()
    {
        yield return new WaitForSeconds(changeSceneDelay);
        SceneManager.LoadScene(tutorialSceneName);
    }

#if UNITY_EDITOR
    [ContextMenu("Delete 'TutorialPlayed' key")]
    public void DeleteTutorialPlayedKey()
    {
        PlayerPrefs.DeleteKey("TutorialPlayed");
    }
#endif
}
