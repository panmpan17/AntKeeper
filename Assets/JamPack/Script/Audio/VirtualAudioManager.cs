using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPack {
    public class VirtualAudioManager : MonoBehaviour
    {
        public static VirtualAudioManager ins;

        public bool dontDestroyOnLoad;

        public AudioSource oneShotAudioSrc;
        protected PrefabPool<AudioOneShotPlayer> oneShotPlayerPool;
        protected float oneShotVolume = 1;

        public AudioSource bgmAudioSrc, secondaryBgmAudioSrc;

        [SerializeField]
        private AudioOneShotPlayer oneShotAudioSourcePrefab;

        private List<OneShotLoopPlayer> loopPlayers = new List<OneShotLoopPlayer>();

        private AudioListener listener;

        public AudioListener Listener {
            get {
                if (listener == null)
                    listener = FindObjectOfType<AudioListener>();
                
                return listener;
            }
        }

        protected virtual void Awake() {
            if (ins != null)
            {
                Destroy(gameObject);
                return;
            }

            ins = this;

            if (oneShotAudioSourcePrefab == null)
            {
                oneShotPlayerPool = new PrefabPool<AudioOneShotPlayer>(delegate {
                    GameObject obj = new GameObject("AudioOneShotPlayer");
                    AudioOneShotPlayer player = obj.AddComponent<AudioOneShotPlayer>();
                    player.Volume = oneShotAudioSrc.volume;
                    return player;
                }, true, "AudioCollects");
            }
            else
            {
                oneShotPlayerPool = new PrefabPool<AudioOneShotPlayer>(oneShotAudioSourcePrefab, true, "AudioCollects");
            }

            if (oneShotAudioSrc == null)
                oneShotAudioSrc = gameObject.AddComponent<AudioSource>();

            if (bgmAudioSrc == null) {
                bgmAudioSrc = gameObject.AddComponent<AudioSource>();
                bgmAudioSrc.loop = true;
            }
            if (secondaryBgmAudioSrc == null) {
                secondaryBgmAudioSrc = gameObject.AddComponent<AudioSource>();
                secondaryBgmAudioSrc.loop = true;
            }

            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }

    #region Audio One Shot
        public void PlayOneShot(AudioPreset preset, float volumeMultiplier=1) {
            oneShotAudioSrc.outputAudioMixerGroup = preset.AudioMixerGroup;
            oneShotAudioSrc.PlayOneShot(
                preset.Clips[Random.Range(0, preset.Clips.Length)],
                preset.DefaultVolume * volumeMultiplier);
        }

        public void PlayOneShot(AudioClip clip, float volumeMultiplier=1) {
            oneShotAudioSrc.PlayOneShot(clip, volumeMultiplier);
        }
    #endregion

    #region Gameobject's Audio One Shot
        public AudioOneShotPlayer PlayOneShotAtPosition(AudioPreset preset, Vector3 position, float volumeMultiplier=1)
        {
            AudioOneShotPlayer player = oneShotPlayerPool.Get();
            player.transform.position = position;
            player.Play(
                preset.Clips[Random.Range(0, preset.Clips.Length)],
                (_player) => oneShotPlayerPool.Put(_player), preset.DefaultVolume * volumeMultiplier);
            return player;
        }

        public AudioOneShotPlayer PlayOneShotAtPosition(AudioClip clip, Vector3 position, float volumeMultiplier=1)
        {
            AudioOneShotPlayer player = oneShotPlayerPool.Get();
            player.transform.position = position;
            player.Play(clip, (_player) => oneShotPlayerPool.Put(_player), volumeMultiplier);
            return player;
        }
    #endregion
    
    #region Background Music
        public void PlayBgm(AudioClip bgmClip, bool overrideCurrentBGM=false) {
            if (bgmAudioSrc.isPlaying) {
                if (!overrideCurrentBGM) return;
                bgmAudioSrc.Stop();
                bgmAudioSrc.clip = null;
            }
            if (secondaryBgmAudioSrc.isPlaying) {
                if (!overrideCurrentBGM) return;
                secondaryBgmAudioSrc.Stop();
                secondaryBgmAudioSrc.clip = null;
            }

            bgmAudioSrc.clip = bgmClip;
            bgmAudioSrc.Play();
        }

        public void BlendNewBgm(AudioClip bgmClip, float fadeOut=0.5f, float fadeOutDelay=0,
                                float fadeIn=0.5f, float fadeInDelay=0.25f) {
            if (!bgmAudioSrc.isPlaying && !secondaryBgmAudioSrc.isPlaying) {
                PlayBgm(bgmClip);
                return;
            }
            // if (bgmAudioSrc.isPlaying && secondaryBgmAudioSrc.isPlaying)
            // TODO: Handle if two bgm audio source both playing

            if (bgmAudioSrc.isPlaying) {
                secondaryBgmAudioSrc.clip = bgmClip;
                StartCoroutine(FadeAudioSource(bgmAudioSrc, 0, fadeOut, fadeOutDelay, stopAfterFade: true));
                secondaryBgmAudioSrc.volume = 0;
                StartCoroutine(FadeAudioSource(secondaryBgmAudioSrc, bgmAudioSrc.volume, fadeIn, fadeInDelay, playerAfterDelay: true, returnVolume: false));
            }
            else {
                bgmAudioSrc.clip = bgmClip;
                StartCoroutine(FadeAudioSource(secondaryBgmAudioSrc, 0, fadeOut, fadeOutDelay, stopAfterFade: true));
                bgmAudioSrc.volume = 0;
                StartCoroutine(FadeAudioSource(bgmAudioSrc, secondaryBgmAudioSrc.volume, fadeIn, fadeInDelay, playerAfterDelay: true, returnVolume: false));
            }
        }
    #endregion

    #region Audio Source Fadeout Control
        public IEnumerator FadeAudioSource(AudioSource src, float targetVolume, float fadeTime,
                                    float delayTime=0, bool returnVolume=true, bool playerAfterDelay=false, bool stopAfterFade=false) {
            if (delayTime > 0) yield return new WaitForSeconds(delayTime);
            if (playerAfterDelay) src.Play();

            float time = 0;
            float originVolume = src.volume;

            while (time < fadeTime) {
                yield return null;
                time += Time.deltaTime;
                src.volume = Mathf.Lerp(originVolume, targetVolume, time / fadeTime);
            }

            if (stopAfterFade) src.Stop();
            if (returnVolume) src.volume = originVolume;
            else src.volume = targetVolume;
        }
        #endregion

    #region Volume Change
    public void ChangeBgmVolume(float volume)
    {
        bgmAudioSrc.volume = volume;
        secondaryBgmAudioSrc.volume = volume;
    }

    public void ChangeSoundVolume(float volume)
    {
        oneShotAudioSrc.volume = volume;

        for (int i = 0; i < oneShotPlayerPool.AliveObjs.Count; i++)
            oneShotPlayerPool.AliveObjs[i].Volume = volume;

        for (int i = 0; i < oneShotPlayerPool.PoolObjs.Count; i++)
            oneShotPlayerPool.PoolObjs[i].Volume = volume;
    }
        #endregion


    #region One Shot Loop Player
    private void Update()
    {
        for (int i = 0; i < loopPlayers.Count; i++)
            loopPlayers[i].Update(this);
    }

    public void PlayLoop(OneShotLoopPlayer player)
    {
        loopPlayers.Add(player);
    }

    public void StopLoop(OneShotLoopPlayer player)
    {
        loopPlayers.Remove(player);
    }
    #endregion
    }
}