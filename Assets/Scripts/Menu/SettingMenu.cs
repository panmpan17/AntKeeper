using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MPack;
using TMPro;
using DigitalRuby.Tween;

public class SettingMenu : AbstractMenu
{
    [Header("Audio")]
    [SerializeField]
    private Slider mainVolumeSlider;
    [SerializeField]
    private Slider musicVolumeSlider;
    [SerializeField]
    private Slider soundEffectVolumeSlider;

    [Header("Resolution")]
    [SerializeField]
    private Vector2Int[] avalibleResolution;
    private int currentResolutionIndex;
    [SerializeField]
    private TextMeshProUGUI resolutionText;

    [Header("DisplayType")]
    [SerializeField]
    [LauguageID]
    private int fullscreenLanguageId;
    [SerializeField]
    [LauguageID]
    private int windowLanguageId;
    [SerializeField]
    private LanguageText displayTypeLanguageText;

    protected override void Awake()
    {
        base.Awake();

        if (PlayerPrefs.HasKey(AudioVolumeAdjust.MainVolumeParameterKey))
            mainVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(AudioVolumeAdjust.MainVolumeParameterKey));
        else
            mainVolumeSlider.SetValueWithoutNotify(0.5f);
        if (PlayerPrefs.HasKey(AudioVolumeAdjust.MusicVolumeParameterKey))
            musicVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(AudioVolumeAdjust.MusicVolumeParameterKey));
        else
            musicVolumeSlider.SetValueWithoutNotify(0.5f);
        if (PlayerPrefs.HasKey(AudioVolumeAdjust.SoundEffectVolumeParameterKey))
            soundEffectVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(AudioVolumeAdjust.SoundEffectVolumeParameterKey));
        else
            soundEffectVolumeSlider.SetValueWithoutNotify(0.5f);
    }

    protected override void FadeInFinished(ITween<float> tweenData)
    {
        base.FadeInFinished(tweenData);

        displayTypeLanguageText.ChangeId(Screen.fullScreen ? fullscreenLanguageId: windowLanguageId);

        float width = Screen.width;
        float height = Screen.height;
        resolutionText.text = string.Format("{0} x {1}", width, height);

        for (int i = 0; i < avalibleResolution.Length; i++)
        {
            if (width >= avalibleResolution[i].x)
                currentResolutionIndex = i;
        }
    }

    public void SwitchDisplayType()
    {
        if (Screen.fullScreen)
        {
            Screen.fullScreen = false;
            displayTypeLanguageText.ChangeId( windowLanguageId);
        }
        else
        {
            Screen.fullScreen = true;
            displayTypeLanguageText.ChangeId(fullscreenLanguageId);
        }
    }

    public void DownSizeResolution()
    {
        if (currentResolutionIndex <= 0)
            return;

        Vector2Int resolution = avalibleResolution[--currentResolutionIndex];
        resolutionText.text = string.Format("{0} x {1}", resolution.x, resolution.y);
        Screen.SetResolution(resolution.x, resolution.y, Screen.fullScreen);
    }

    public void UpSizeResolution()
    {
        if (currentResolutionIndex >= avalibleResolution.Length - 1)
            return;

        Vector2Int resolution = avalibleResolution[++currentResolutionIndex];
        resolutionText.text = string.Format("{0} x {1}", resolution.x, resolution.y);
        Screen.SetResolution(resolution.x, resolution.y, Screen.fullScreen);
    }
}
