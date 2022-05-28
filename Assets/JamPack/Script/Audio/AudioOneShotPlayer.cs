using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPack {
    public class AudioOneShotPlayer : MonoBehaviour, IPoolableObj
    {
        AudioSource audioSource;

        private float volume = 1, volumeMultiplier = 1;
        public float Volume {
            set {
                volume = value;
                if (audioSource != null) audioSource.volume = value * volumeMultiplier;
            }
        }

        public System.Action<AudioOneShotPlayer> PlayEndCall;
        private OneShotLoopPlayer loopPlayer;

        public void Instantiate() {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.spatialBlend = 1;
            gameObject.SetActive(true);
        }
        public void DeactivateObj(Transform collectionTransform) {
            if (collectionTransform != null) transform.SetParent(collectionTransform);
            gameObject.SetActive(false);
        }
        public void Reinstantiate() {
            transform.SetParent(null);
            gameObject.SetActive(true);
            enabled = true;
        }

        public void Play(AudioClip clip, System.Action<AudioOneShotPlayer> playEndCall=null, float _volumeMultiplier=1) {
            audioSource.clip = clip;
            audioSource.volume = volume * _volumeMultiplier;
            volumeMultiplier = _volumeMultiplier;
            audioSource.Play();
            audioSource.loop = false;

            PlayEndCall = playEndCall;
        }

        public void RegisterForceStop(OneShotLoopPlayer _loopPlayer) {
            loopPlayer = _loopPlayer;
            loopPlayer.ForceStopDelegate += Stop;
        }

        public void Stop() {
            audioSource.Stop();
        }

        private void Update() {
            if (!audioSource.isPlaying) {
                if (PlayEndCall != null) PlayEndCall(this);
                if (loopPlayer != null)
                    loopPlayer.ForceStopDelegate -= Stop;
                enabled = false;
            }
        }
    }
}