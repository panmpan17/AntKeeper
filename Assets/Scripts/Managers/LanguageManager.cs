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
        if (isEnglish) SwitchToEnglish();
        else SwitchToChinese();
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
