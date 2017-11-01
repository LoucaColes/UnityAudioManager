using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SFXData
{
    public string m_name;
    public AudioClip m_audioClip;

    public bool m_looping;

    [Range(0f, 1f)]
    public float m_volume;

    [Range(0.3f, 3f)]
    public float m_pitch;

    [HideInInspector]
    public GameObject m_object;

    [HideInInspector]
    public AudioSource m_audioSource;
}

public class AudioManager : MonoBehaviour
{
    public SFXData m_backgroundMusic;

    public SFXData[] m_sfxDataList;

    public static AudioManager m_instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        CreateInstance();

        SetUpMusic();

        SetUpSFX();
    }

    private void Start()
    {
        PlayMusic();
    }

    private void SetUpMusic()
    {
        GameObject t_child = new GameObject(m_backgroundMusic.m_name);
        t_child.transform.parent = transform;

        AudioSource t_audioSource = t_child.AddComponent<AudioSource>();

        m_backgroundMusic.m_object = t_child;
        m_backgroundMusic.m_audioSource = t_audioSource;

        m_backgroundMusic.m_audioSource.clip = m_backgroundMusic.m_audioClip;
        m_backgroundMusic.m_audioSource.volume = m_backgroundMusic.m_volume;
        m_backgroundMusic.m_audioSource.pitch = m_backgroundMusic.m_pitch;
        m_backgroundMusic.m_audioSource.loop = m_backgroundMusic.m_looping;
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
        for (int i = 0; i < m_sfxDataList.Length; i++)
        {
            GameObject t_child = new GameObject(m_sfxDataList[i].m_name);
            t_child.transform.parent = transform;

            AudioSource t_audioSource = t_child.AddComponent<AudioSource>();

            m_sfxDataList[i].m_object = t_child;
            m_sfxDataList[i].m_audioSource = t_audioSource;

            m_sfxDataList[i].m_audioSource.clip = m_sfxDataList[i].m_audioClip;
            m_sfxDataList[i].m_audioSource.volume = m_sfxDataList[i].m_volume;
            m_sfxDataList[i].m_audioSource.pitch = m_sfxDataList[i].m_pitch;
            m_sfxDataList[i].m_audioSource.loop = m_sfxDataList[i].m_looping;
        }
    }

    public void PlayMusic()
    {
        Debug.Log("Playing Music");
        m_backgroundMusic.m_audioSource.Play();
    }

    public void PlaySFX(string _name)
    {
        SFXData t_data = Array.Find(m_sfxDataList, sfx => sfx.m_name == _name);
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
}