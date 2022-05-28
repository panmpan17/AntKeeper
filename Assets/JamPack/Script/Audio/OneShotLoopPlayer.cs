#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MPack
{
    [System.Serializable]
    public class OneShotLoopPlayer
    {
        [SerializeField]
        private AudioPreset preset;
        [SerializeField]
        private Timer Interval;
        // [SerializeField]
        // private bool LoopAfterSoundFinished;
        public float Volume = 1;

        [System.NonSerialized]
        public Vector3 Position;
        private bool usePosition;

        public System.Action ForceStopDelegate;

        private AudioOneShotPlayer player;

        private bool isPlaying = false;
        public bool IsPlaying {
            get {
                return isPlaying;
            }
        }

        public void Play(VirtualAudioManager audioMgr)
        {
            isPlaying = true;
            Interval.Reset();
            usePosition = false;

            audioMgr.PlayOneShot(preset, Volume);
            audioMgr.PlayLoop(this);
        }

        public void Play(VirtualAudioManager audioMgr, Vector3 position)
        {
            isPlaying = true;
            Interval.Reset();
            usePosition = true;
            Position = position;

            player = audioMgr.PlayOneShotAtPosition(preset, position, Volume);
            if (player != null)
                player.RegisterForceStop(this);
            audioMgr.PlayLoop(this);
        }

        public void Stop(VirtualAudioManager audioMgr)
        {
            isPlaying = false;
            audioMgr.StopLoop(this);

            ForceStopDelegate?.Invoke();
            ForceStopDelegate = null;
            if (player != null) {
                player.Stop();
                player = null;
            }
        }

        public void Update(VirtualAudioManager audioMgr)
        {
            if (Interval.UpdateEnd)
            {
                Interval.Reset();
                if (usePosition)
                {
                    AudioOneShotPlayer player = audioMgr.PlayOneShotAtPosition(preset, Position, Volume);
                    if (player != null)
                        player.RegisterForceStop(this);
                }
                else
                    audioMgr.PlayOneShot(preset, Volume);
            }
        }
    }
}