using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using MPack;

public class AudioVolumeAdjust : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField]
    private AnimationCurve volumeCurve;
    [SerializeField]
    private RangeReference volumeRange;

    [SerializeField]
    private EventReference initialMainVolumeEvent;
    [SerializeField]
    private EventReference initialMusicVolumeEvent;
    [SerializeField]
    private EventReference initialSoundEffectVolumeEvent;
    
    [SerializeField]
    private string mainVolumeParameterKey = "MainVolume";
    [SerializeField]
    private string musicVolumeParameterKey = "MusicVolume";
    [SerializeField]
    private string soundEffectVolumeParameterKey = "SoundEffectVolume";

    void Start()
    {
        float mainVolume = 0.5f;
        float musicVolume = 0.5f;
        float soundEffectVolume = 0.5f;

        if (PlayerPrefs.HasKey(mainVolumeParameterKey)) mainVolume = PlayerPrefs.GetFloat(mainVolumeParameterKey);
        if (PlayerPrefs.HasKey(musicVolumeParameterKey)) musicVolume = PlayerPrefs.GetFloat(musicVolumeParameterKey);
        if (PlayerPrefs.HasKey(soundEffectVolumeParameterKey)) soundEffectVolume = PlayerPrefs.GetFloat(soundEffectVolumeParameterKey);

        audioMixer.SetFloat(mainVolumeParameterKey, volumeRange.Lerp(volumeCurve.Evaluate(mainVolume)));
        audioMixer.SetFloat(musicVolumeParameterKey, volumeRange.Lerp(volumeCurve.Evaluate(musicVolume)));
        audioMixer.SetFloat(soundEffectVolumeParameterKey, volumeRange.Lerp(volumeCurve.Evaluate(soundEffectVolume)));

        initialMainVolumeEvent.Invoke(mainVolume);
        initialMusicVolumeEvent.Invoke(musicVolume);
        initialSoundEffectVolumeEvent.Invoke(soundEffectVolume);
    }

    public void SetMainVolume(float volume)
    {
        PlayerPrefs.SetFloat(mainVolumeParameterKey, volume);
        PlayerPrefs.Save();

        volume = volumeRange.Lerp(volumeCurve.Evaluate(volume));
        audioMixer.SetFloat(mainVolumeParameterKey, volume);
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat(musicVolumeParameterKey, volume);
        PlayerPrefs.Save();

        volume = volumeRange.Lerp(volumeCurve.Evaluate(volume));
        audioMixer.SetFloat(musicVolumeParameterKey, volume);
    }

    public void SetSoundEffectVolume(float volume)
    {
        PlayerPrefs.SetFloat(soundEffectVolumeParameterKey, volume);
        PlayerPrefs.Save();

        volume = volumeRange.Lerp(volumeCurve.Evaluate(volume));
        audioMixer.SetFloat(soundEffectVolumeParameterKey, volume);
    }
}
