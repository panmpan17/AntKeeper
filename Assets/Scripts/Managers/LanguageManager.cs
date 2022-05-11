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

    void Awake()
    {
        SwitchToEnglish();
    }

    public void SwitchToEnglish()
    {
        LanguageMgr.AssignLanguageData(english);
    }

    public void SwitchToChinese()
    {
        LanguageMgr.AssignLanguageData(chinese);
    }

    public void Switch(bool boolean)
    {
        if (boolean) SwitchToEnglish();
        else SwitchToChinese();
    }
}
