using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class LanguageManager : MonoBehaviour
{
    [SerializeField]
    private LanguageData english;
    [SerializeField]
    private LanguageData chinese;

    [SerializeField]
    private bool isEnglish = true;

    void Awake()
    {
        if (PlayerPrefs.HasKey("Language"))
            isEnglish = PlayerPrefs.GetInt("Language") == english.ID;
        else
        {
            switch(Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                    isEnglish = false;
                    break;
            }
        }

        if (isEnglish) SwitchToEnglish();
        else SwitchToChinese();
    }

    public void SwitchToEnglish()
    {
        LanguageMgr.AssignLanguageData(english);
        PlayerPrefs.SetInt("Language", english.ID);
    }

    public void SwitchToChinese()
    {
        LanguageMgr.AssignLanguageData(chinese);
        PlayerPrefs.SetInt("Language", chinese.ID);
    }

    public void Switch(bool boolean)
    {
        isEnglish = boolean;
        if (boolean) SwitchToEnglish();
        else SwitchToChinese();
    }

    public void Switch()
    {
        isEnglish = !isEnglish;
        if (isEnglish) SwitchToEnglish();
        else SwitchToChinese();
    }
}
