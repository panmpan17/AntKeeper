using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using MPack;

public class AudioVolumeAdjust : MonoBehaviour
{
    public const string MainVolumeParameterKey = "MainVolume";
    public const string MusicVolumeParameterKey = "MusicVolume";
    public const string SoundEffectVolumeParameterKey = "SoundEffectVolume";

    [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField]
    private AnimationCurve volumeCurve;
    [SerializeField]
    private RangeReference volumeRange;

    void Start()
    {
        float mainVolume = 0.5f;
        float musicVolume = 0.5f;
        float soundEffectVolume = 0.5f;

        if (PlayerPrefs.HasKey(MainVolumeParameterKey)) mainVolume = PlayerPrefs.GetFloat(MainVolumeParameterKey);
        if (PlayerPrefs.HasKey(MusicVolumeParameterKey)) musicVolume = PlayerPrefs.GetFloat(MusicVolumeParameterKey);
        if (PlayerPrefs.HasKey(SoundEffectVolumeParameterKey)) soundEffectVolume = PlayerPrefs.GetFloat(SoundEffectVolumeParameterKey);

        audioMixer.SetFloat(MainVolumeParameterKey, volumeRange.Lerp(volumeCurve.Evaluate(mainVolume)));
        audioMixer.SetFloat(MusicVolumeParameterKey, volumeRange.Lerp(volumeCurve.Evaluate(musicVolume)));
        audioMixer.SetFloat(SoundEffectVolumeParameterKey, volumeRange.Lerp(volumeCurve.Evaluate(soundEffectVolume)));
    }

    public void SetMainVolume(float volume)
    {
        PlayerPrefs.SetFloat(MainVolumeParameterKey, volume);
        PlayerPrefs.Save();

        audioMixer.SetFloat(MainVolumeParameterKey, volumeRange.Lerp(volumeCurve.Evaluate(volume)));
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat(MusicVolumeParameterKey, volume);
        PlayerPrefs.Save();

        audioMixer.SetFloat(MusicVolumeParameterKey, volumeRange.Lerp(volumeCurve.Evaluate(volume)));
    }

    public void SetSoundEffectVolume(float volume)
    {
        PlayerPrefs.SetFloat(SoundEffectVolumeParameterKey, volume);
        PlayerPrefs.Save();

        audioMixer.SetFloat(SoundEffectVolumeParameterKey, volumeRange.Lerp(volumeCurve.Evaluate(volume)));
    }
}
