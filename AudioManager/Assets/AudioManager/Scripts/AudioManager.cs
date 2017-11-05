using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioManager
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager m_instance;

        [SerializeField]
        private List<GameObject> m_catogories;

        public AudioData[] m_backgroundMusicList;

        public AudioData[] m_sfxList;

        public AudioData[] m_voiceList;

        [SerializeField]
        private AudioData m_currentMusic;

        [SerializeField]
        private AudioData m_prevMusic;

        [Range(0, 1), SerializeField]
        private float m_sfxGlobalVolume = 1f;

        [Range(0, 1), SerializeField]
        private float m_musicGlobalVolume = 1f;

        [Range(0, 1), SerializeField]
        private float m_voiceGlobalVolume = 1f;

        [Range(0, 1), SerializeField]
        private float m_masterVolume = 1f;

        private void Awake()
        {
            m_catogories = new List<GameObject>();

            DontDestroyOnLoad(gameObject);

            CreateInstance();

            SetUpAudioArray(m_backgroundMusicList, "Background Music");

            SetUpAudioArray(m_sfxList, "SFX");

            SetUpAudioArray(m_voiceList, "Voice Lines");

            ClearCurrentPrevMusic();
        }

        private void CreateInstance()
        {
            if (!m_instance)
            {
                m_instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void SetUpAudioArray(AudioData[] _array, string _arrayName)
        {
            GameObject t_catergory = new GameObject(_arrayName);
            t_catergory.transform.parent = transform;
            m_catogories.Add(t_catergory);

            for (int i = 0; i < _array.Length; i++)
            {
                GameObject t_child = new GameObject(_array[i].m_name);
                t_child.transform.parent = t_catergory.transform;

                AudioSource t_audioSource = t_child.AddComponent<AudioSource>();

                _array[i].m_object = t_child;
                _array[i].m_audioSource = t_audioSource;

                _array[i].m_audioSource.clip = _array[i].m_audioClip;
                _array[i].SetOriginalVolume(_array[i].m_volume);
                _array[i].m_volume = UpdateAudioDataVolume(_array[i]);
                _array[i].m_audioSource.volume = _array[i].m_volume;
                _array[i].m_audioSource.pitch = _array[i].m_pitch;
                _array[i].m_audioSource.loop = _array[i].m_looping;
            }
        }

        private void UpdateArrayVolume(AudioData[] _array)
        {
            for (int i = 0; i < _array.Length; i++)
            {
                _array[i].m_volume = UpdateAudioDataVolume(_array[i]);
                _array[i].m_audioSource.volume = _array[i].m_volume;
            }
        }

        private float UpdateAudioDataVolume(AudioData _audioData)
        {
            if (_audioData.m_type == AudioData.AudioType.MUSIC)
            {
                return _audioData.GetOriginalVolume() * m_musicGlobalVolume * m_masterVolume;
            }
            else if (_audioData.m_type == AudioData.AudioType.SFX)
            {
                return _audioData.GetOriginalVolume() * m_sfxGlobalVolume * m_masterVolume;
            }
            else
            {
                return _audioData.GetOriginalVolume() * m_voiceGlobalVolume * m_masterVolume;
            }
        }

        private void ClearCurrentPrevMusic()
        {
            m_currentMusic = null;
            m_prevMusic = m_currentMusic;
        }

        public void PlayMusic(string _name)
        {
            Debug.Log("Playing Music");
            AudioData t_data = Array.Find(m_backgroundMusicList, bgm => bgm.m_name == _name);
            if (t_data == null)
            {
                Debug.LogError("Didnt find music");
                return;
            }
            else
            {
                m_prevMusic = m_currentMusic;
                m_currentMusic = t_data;
                PlayNextMusicTrack();
            }
        }

        public void PlayMusic(int _id)
        {
            if (_id >= 0 && _id < m_backgroundMusicList.Length)
            {
                Debug.Log("Playing Music");

                m_prevMusic = m_currentMusic;
                m_currentMusic = m_backgroundMusicList[_id];

                PlayNextMusicTrack();
            }
            else
            {
                Debug.LogError("Didnt find music");
                return;
            }
        }

        private void PlayNextMusicTrack()
        {
            m_currentMusic.m_audioSource.Play();
            if (!m_currentMusic.m_fade && m_prevMusic.m_audioSource != null)
            {
                m_prevMusic.m_audioSource.Stop();
            }
            if (m_currentMusic.m_fade)
            {
                StartCoroutine(FadeIn(m_currentMusic));
                if (m_prevMusic.m_audioSource != null)
                {
                    StartCoroutine(FadeOut(m_prevMusic));
                }
            }
        }

        public void PlaySFX(string _name)
        {
            AudioData t_data = Array.Find(m_sfxList, sfx => sfx.m_name == _name);
            if (t_data == null)
            {
                Debug.LogError("Didnt find sound");
                return;
            }
            else
            {
                t_data.m_audioSource.Play();
            }
        }

        public void PlaySFX(int _id)
        {
            if (_id >= 0 && _id < m_sfxList.Length)
            {
                Debug.Log("Playing sound");
                m_sfxList[_id].m_audioSource.Play();
            }
            else
            {
                Debug.LogError("Didnt find sound");
                return;
            }
        }

        public void PlayVoice(string _name)
        {
            AudioData t_data = Array.Find(m_voiceList, voice => voice.m_name == _name);
            if (t_data == null)
            {
                Debug.LogError("Didnt find voice");
                return;
            }
            else
            {
                t_data.m_audioSource.Play();
            }
        }

        public void PlayVoice(int _id)
        {
            if (_id >= 0 && _id < m_voiceList.Length)
            {
                Debug.Log("Playing voice");
                m_voiceList[_id].m_audioSource.Play();
            }
            else
            {
                Debug.LogError("Didnt find voice");
                return;
            }
        }

        private IEnumerator FadeIn(AudioData _audioData)
        {
            _audioData.m_audioSource.volume = 0;
            float t_volume = _audioData.m_audioSource.volume;

            while (_audioData.m_audioSource.volume < _audioData.m_volume)
            {
                t_volume += _audioData.m_fadeInSpeed;
                _audioData.m_audioSource.volume = t_volume;
                yield return new WaitForSeconds(0.1f);
            }
        }

        private IEnumerator FadeOut(AudioData _audioData)
        {
            float t_volume = _audioData.m_audioSource.volume;

            while (_audioData.m_audioSource.volume > 0)
            {
                t_volume -= _audioData.m_fadeOutSpeed;
                _audioData.m_audioSource.volume = t_volume;
                yield return new WaitForSeconds(0.1f);
            }
            if (_audioData.m_audioSource.volume == 0)
            {
                _audioData.m_audioSource.Stop();
                _audioData.m_audioSource.volume = _audioData.m_volume;
            }
        }

        public void SetSfxGlobalVolume(float _volume)
        {
            if (_volume >= 0 && _volume <= 1)
            {
                m_sfxGlobalVolume = _volume;
                UpdateArrayVolume(m_sfxList);
            }
        }

        public void SetMusicGlobalVolume(float _volume)
        {
            if (_volume >= 0 && _volume <= 1)
            {
                m_musicGlobalVolume = _volume;
                UpdateArrayVolume(m_backgroundMusicList);
            }
        }

        public void SetVoiceGlobalVolume(float _volume)
        {
            if (_volume >= 0 && _volume <= 1)
            {
                m_voiceGlobalVolume = _volume;
                UpdateArrayVolume(m_voiceList);
            }
        }

        public void SetMasterVolume(float _volume)
        {
            if (_volume >= 0 && _volume <= 1)
            {
                m_masterVolume = _volume;
                UpdateArrayVolume(m_sfxList);
                UpdateArrayVolume(m_backgroundMusicList);
                UpdateArrayVolume(m_voiceList);
            }
        }

        public void UpdateGlobalVolumes(float _sfxVolume, float _musicVolume, float _masterVolume)
        {
            if (_sfxVolume >= 0 && _sfxVolume <= 1)
            {
                m_sfxGlobalVolume = _sfxVolume;
            }
            if (_musicVolume >= 0 && _musicVolume <= 1)
            {
                m_musicGlobalVolume = _musicVolume;
            }
            if (_masterVolume >= 0 && _masterVolume <= 1)
            {
                m_masterVolume = _masterVolume;
            }

            UpdateArrayVolume(m_sfxList);
            UpdateArrayVolume(m_backgroundMusicList);
            UpdateArrayVolume(m_voiceList);
        }
    }
}