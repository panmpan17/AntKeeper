using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class LanguageManager : MonoBehaviour
{
    public const string LanguagePreferenceKey = "Language";

    [SerializeField]
    private LanguageData english;
    [SerializeField]
    private LanguageData chinese;

    [SerializeField]
    private bool isEnglish = true;

    void Awake()
    {
        if (PlayerPrefs.HasKey(LanguagePreferenceKey))
            isEnglish = PlayerPrefs.GetInt(LanguagePreferenceKey) == english.ID;
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
        PlayerPrefs.SetInt(LanguagePreferenceKey, english.ID);
    }

    public void SwitchToChinese()
    {
        LanguageMgr.AssignLanguageData(chinese);
        PlayerPrefs.SetInt(LanguagePreferenceKey, chinese.ID);
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

#if UNITY_EDITOR
    [ContextMenu("Reset Preference")]
    public void ResetLanguagePreference()
    {
        PlayerPrefs.DeleteKey(LanguagePreferenceKey);
    }
#endif
}
