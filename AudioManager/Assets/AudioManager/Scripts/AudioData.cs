using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioManager
{
    // Customisable class that provides an easy way to interact
    // with an audio source using the audio manager script
    [System.Serializable]
    public class AudioData
    {
        // Name of the sound
        public string m_name;

        // Enum of the types of audio this system uses
        public enum AudioType
        {
            SFX,
            MUSIC,
            VOICE,
        }

        // The audio type of this specific sound
        public AudioType m_type;

        // The audio clip of the sound
        public AudioClip m_audioClip;

        // Make the sound loop (true means it loops, false means
        // it doesn't)
        public bool m_looping;

        // The current volume of the sound
        [Range(0f, 1f)]
        public float m_volume;

        // The original volume of the sound
        [Range(0f, 1f), SerializeField]
        private float m_origVolume;

        // The priority of the sound
        [Range(0f, 256f)]
        public int m_priority;

        // The pitch of the sound
        [Range(0.3f, 3f)]
        public float m_pitch;

        // The pan stereo of the sound (-1 means sound plays
        // from left speaker, 1 means sound plays from the
        // right speaker, 0 means sound plays from both
        [Range(-1f, 1f)]
        public float m_panStereo;

        // Whether the sound is being played in 2D or 3D
        [Range(0f, 1f)]
        public float m_spatialBlend;

        // How fast the sound fades in
        [Range(0f, 5f)]
        public float m_fadeInSpeed;

        // How fast the sound fades out
        [Range(0f, 5f)]
        public float m_fadeOutSpeed;

        // Will the sound fade (true means it will and
        // false means it won't)
        public bool m_fade;

        // How long the sound will wait before playing
        // This will override any fade settings
        public float m_delayTime;

        // Will the volume be randomised before playing
        // (true means it will and false means it won't)
        // This is ignored with music and voices
        public bool m_randVolume;

        // Will the pitch be randomised before playing
        // (true means it will and false means it won't)
        // This is ignored with music and voices
        public bool m_randPitch;

        // Will the delay be randomised before playing
        // (true means it will and false means it won't)
        // This is ignored with music and voices
        public bool m_randDelay;

        // The object that will play the specific sound
        // and used for 3D sound
        [HideInInspector]
        public GameObject m_object;

        // The audio source of the sound and it's game
        // object
        [HideInInspector]
        public AudioSource m_audioSource;

        // Function to set the original sound when
        // setting up the sound object
        public void SetOriginalVolume(float _volume)
        {
            m_origVolume = _volume;
        }

        // Function to get the original sound when
        // updating the volume of the sound object
        public float GetOriginalVolume()
        {
            return m_origVolume;
        }
    }
}