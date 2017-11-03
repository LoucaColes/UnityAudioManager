using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManager;

public class TestInput : MonoBehaviour
{
    public int _id;
    public string _name;

    [Range(0, 1)]
    public float m_sfxGlobalVolume;

    [Range(0, 1)]
    public float m_musicGlobalVolume;

    [Range(0, 1)]
    public float m_masterVolume;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            AudioManager.AudioManager.m_instance.PlayMusic(_id);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            AudioManager.AudioManager.m_instance.PlaySFX(_id);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            AudioManager.AudioManager.m_instance.SetSfxGlobalVolume(m_sfxGlobalVolume);
            AudioManager.AudioManager.m_instance.SetMusicGlobalVolume(m_musicGlobalVolume);
            AudioManager.AudioManager.m_instance.SetMasterVolume(m_masterVolume);
        }
    }
}