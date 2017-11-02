using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioManager
{
    public class AudioManager : MonoBehaviour
    {
        public AudioData[] m_backgroundMusicList;

        public AudioData[] m_sfxList;

        public static AudioManager m_instance;

        [SerializeField]
        private AudioData m_currentMusic;

        [SerializeField]
        private AudioData m_prevMusic;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            CreateInstance();

            SetUpMusic();

            SetUpSFX();
        }

        private void Start()
        {
        }

        private void Update()
        {
        }

        private void SetUpMusic()
        {
            Debug.Log("Setting Up Music");
            for (int i = 0; i < m_backgroundMusicList.Length; i++)
            {
                GameObject t_child = new GameObject(m_backgroundMusicList[i].m_name);
                t_child.transform.parent = transform;

                AudioSource t_audioSource = t_child.AddComponent<AudioSource>();

                m_backgroundMusicList[i].m_object = t_child;
                m_backgroundMusicList[i].m_audioSource = t_audioSource;

                m_backgroundMusicList[i].m_audioSource.clip = m_backgroundMusicList[i].m_audioClip;
                m_backgroundMusicList[i].m_audioSource.volume = m_backgroundMusicList[i].m_volume;
                m_backgroundMusicList[i].m_audioSource.pitch = m_backgroundMusicList[i].m_pitch;
                m_backgroundMusicList[i].m_audioSource.loop = m_backgroundMusicList[i].m_looping;
            }
            m_currentMusic = m_backgroundMusicList[0];
            m_prevMusic = m_currentMusic;
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

        private void SetUpSFX()
        {
            for (int i = 0; i < m_sfxList.Length; i++)
            {
                GameObject t_child = new GameObject(m_sfxList[i].m_name);
                t_child.transform.parent = transform;

                AudioSource t_audioSource = t_child.AddComponent<AudioSource>();

                m_sfxList[i].m_object = t_child;
                m_sfxList[i].m_audioSource = t_audioSource;

                m_sfxList[i].m_audioSource.clip = m_sfxList[i].m_audioClip;
                m_sfxList[i].m_audioSource.volume = m_sfxList[i].m_volume;
                m_sfxList[i].m_audioSource.pitch = m_sfxList[i].m_pitch;
                m_sfxList[i].m_audioSource.loop = m_sfxList[i].m_looping;
            }
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
                if (m_currentMusic != null && t_data.m_fadeInTime == 0)
                {
                    m_prevMusic = m_currentMusic;
                    m_currentMusic.m_audioSource.Stop();
                }
                t_data.m_audioSource.Play();
                m_currentMusic = t_data;
            }
        }

        public void PlayMusic(int _id)
        {
            if (_id >= 0 && _id < m_backgroundMusicList.Length)
            {
                Debug.Log("Playing Music");
                if (m_currentMusic != null && m_backgroundMusicList[_id].m_fadeInTime == 0)
                {
                    m_prevMusic = m_currentMusic;
                    m_currentMusic.m_audioSource.Stop();
                }
                m_backgroundMusicList[_id].m_audioSource.Play();
                m_currentMusic = m_backgroundMusicList[_id];
            }
            else
            {
                Debug.LogError("Didnt find music");
                return;
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
    }
}