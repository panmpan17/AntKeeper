using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace MPack {
    [CreateAssetMenu(menuName="MPack/AudioPreset")]
    public class AudioPreset : ScriptableObject
    {
        [Range(0, 1)]
        public float DefaultVolume = 1;
        public AudioClip[] Clips;
        public AudioMixerGroup AudioMixerGroup;
    }
}