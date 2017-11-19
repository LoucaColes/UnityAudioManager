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
        private List<GameObject> m_categories;

        public AudioData[] m_backgroundMusicList;

        public AudioData[] m_sfxList;

        public AudioData[] m_voiceList;

        [SerializeField]
        private AudioData m_currentMusic;

        [SerializeField]
        private AudioData m_prevMusic;

        [SerializeField]
        private List<AudioData> m_currentSFX;

        [SerializeField]
        private List<AudioData> m_currentVoices;

        [Range(0, 1), SerializeField]
        private float m_sfxGlobalVolume = 1f;

        [Range(0, 1), SerializeField]
        private float m_musicGlobalVolume = 1f;

        [Range(0, 1), SerializeField]
        private float m_voiceGlobalVolume = 1f;

        [Range(0, 1), SerializeField]
        private float m_masterVolume = 1f;

        [SerializeField]
        private Vector2 m_volumeMinMax;

        [SerializeField]
        private Vector2 m_pitchMinMax;

        private void Awake()
        {
            m_categories = new List<GameObject>();
            m_currentSFX = new List<AudioData>();
            m_currentVoices = new List<AudioData>();

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
            m_categories.Add(t_catergory);

            for (int i = 0; i < _array.Length; i++)
            {
                GameObject t_child = new GameObject(_array[i].m_name);
                t_child.transform.parent = t_catergory.transform;

                AudioSource t_audioSource = t_child.AddComponent<AudioSource>();

                _array[i].m_object = t_child;
                _array[i].m_audioSource = t_audioSource;

                _array[i].m_audioSource.clip = _array[i].m_audioClip;
                UpdateData(_array, i);
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

        public void PlayMusic(string _name, Vector3 _pos)
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
                t_data.m_object.transform.position = _pos;
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

        public void PlayMusic(int _id, Vector3 _pos)
        {
            if (_id >= 0 && _id < m_backgroundMusicList.Length)
            {
                Debug.Log("Playing Music");

                m_prevMusic = m_currentMusic;
                m_currentMusic = m_backgroundMusicList[_id];
                m_currentMusic.m_object.transform.position = _pos;
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
            if (m_currentMusic.m_delayTime > 0 || m_currentMusic.m_randDelay)
            {
                float t_delayTime = m_currentMusic.m_delayTime;
                if (m_currentMusic.m_randDelay)
                {
                    t_delayTime = UnityEngine.Random.Range(0.1f, 1f);
                }
                m_currentMusic.m_audioSource.PlayDelayed(t_delayTime);
            }
            else
            {
                m_currentMusic.m_audioSource.Play();
                if (!m_currentMusic.m_fade && m_prevMusic != null)
                {
                    m_prevMusic.m_audioSource.Stop();
                }
                if (m_currentMusic.m_fade)
                {
                    StartCoroutine(FadeIn(m_currentMusic));
                    if (m_prevMusic != null)
                    {
                        StartCoroutine(FadeOut(m_prevMusic));
                    }
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
                PlayFoundSFX(t_data);
            }
        }

        private void PlayFoundSFX(AudioData t_data)
        {
            if (t_data.m_randVolume)
            {
                t_data.m_audioSource.volume = UnityEngine.Random.Range(0.1f, 1);
            }
            if (t_data.m_randPitch)
            {
                t_data.m_audioSource.pitch = UnityEngine.Random.Range(0.3f, 3);
            }
            if (t_data.m_delayTime == 0)
            {
                t_data.m_audioSource.Play();
            }
            else if (t_data.m_delayTime > 0 || t_data.m_randDelay)
            {
                float t_delayTime = t_data.m_delayTime;
                if (t_data.m_randDelay)
                {
                    t_delayTime = UnityEngine.Random.Range(0.1f, 1f);
                }
                t_data.m_audioSource.PlayDelayed(t_delayTime);
            }
            m_currentSFX.Add(t_data);
        }

        public void PlaySFX(string _name, Vector3 _pos)
        {
            AudioData t_data = Array.Find(m_sfxList, sfx => sfx.m_name == _name);
            if (t_data == null)
            {
                Debug.LogError("Didnt find sound");
                return;
            }
            else
            {
                t_data.m_object.transform.position = _pos;
                PlayFoundSFX(t_data);
            }
        }

        public void PlaySFX(int _id)
        {
            if (_id >= 0 && _id < m_sfxList.Length)
            {
                Debug.Log("Playing sound");
                if (m_sfxList[_id].m_randVolume)
                {
                    m_sfxList[_id].m_audioSource.volume = UnityEngine.Random.Range(0.1f, 1);
                }
                if (m_sfxList[_id].m_randPitch)
                {
                    m_sfxList[_id].m_audioSource.pitch = UnityEngine.Random.Range(0.3f, 3);
                }
                if (m_sfxList[_id].m_delayTime == 0)
                {
                    m_sfxList[_id].m_audioSource.Play();
                }
                else if (m_sfxList[_id].m_delayTime > 0 || m_sfxList[_id].m_randDelay)
                {
                    float t_delayTime = m_sfxList[_id].m_delayTime;
                    if (m_sfxList[_id].m_randDelay)
                    {
                        t_delayTime = UnityEngine.Random.Range(0.1f, 1f);
                    }
                    m_sfxList[_id].m_audioSource.PlayDelayed(t_delayTime);
                }
                m_currentSFX.Add(m_sfxList[_id]);
            }
            else
            {
                Debug.LogError("Didnt find sound");
                return;
            }
        }

        public void PlaySFX(int _id, Vector3 _pos)
        {
            if (_id >= 0 && _id < m_sfxList.Length)
            {
                Debug.Log("Playing sound");
                m_sfxList[_id].m_object.transform.position = _pos;
                if (m_sfxList[_id].m_randVolume)
                {
                    m_sfxList[_id].m_audioSource.volume = UnityEngine.Random.Range(0.1f, 1);
                }
                if (m_sfxList[_id].m_randPitch)
                {
                    m_sfxList[_id].m_audioSource.pitch = UnityEngine.Random.Range(0.3f, 3);
                }
                if (m_sfxList[_id].m_delayTime == 0)
                {
                    m_sfxList[_id].m_audioSource.Play();
                }
                else if (m_sfxList[_id].m_delayTime > 0 || m_sfxList[_id].m_randDelay)
                {
                    float t_delayTime = m_sfxList[_id].m_delayTime;
                    if (m_sfxList[_id].m_randDelay)
                    {
                        t_delayTime = UnityEngine.Random.Range(0.1f, 1f);
                    }
                    m_sfxList[_id].m_audioSource.PlayDelayed(t_delayTime);
                }
                m_currentSFX.Add(m_sfxList[_id]);
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
                if (t_data.m_delayTime == 0)
                {
                    t_data.m_audioSource.Play();
                }
                else if (t_data.m_delayTime > 0 || t_data.m_randDelay)
                {
                    float t_delayTime = t_data.m_delayTime;
                    if (t_data.m_randDelay)
                    {
                        t_delayTime = UnityEngine.Random.Range(0.1f, 1f);
                    }
                    t_data.m_audioSource.PlayDelayed(t_delayTime);
                }
                m_currentVoices.Add(t_data);
            }
        }

        public void PlayVoice(string _name, Vector3 _pos)
        {
            AudioData t_data = Array.Find(m_voiceList, voice => voice.m_name == _name);
            if (t_data == null)
            {
                Debug.LogError("Didnt find voice");
                return;
            }
            else
            {
                t_data.m_object.transform.position = _pos;
                if (t_data.m_delayTime == 0)
                {
                    t_data.m_audioSource.Play();
                }
                else if (t_data.m_delayTime > 0 || t_data.m_randDelay)
                {
                    float t_delayTime = t_data.m_delayTime;
                    if (t_data.m_randDelay)
                    {
                        t_delayTime = UnityEngine.Random.Range(0.1f, 1f);
                    }
                    t_data.m_audioSource.PlayDelayed(t_delayTime);
                }
                m_currentVoices.Add(t_data);
            }
        }

        public void PlayVoice(int _id)
        {
            if (_id >= 0 && _id < m_voiceList.Length)
            {
                Debug.Log("Playing voice");
                if (m_voiceList[_id].m_delayTime == 0)
                {
                    m_voiceList[_id].m_audioSource.Play();
                }
                else if (m_voiceList[_id].m_delayTime > 0 || m_voiceList[_id].m_randDelay)
                {
                    float t_delayTime = m_voiceList[_id].m_delayTime;
                    if (m_voiceList[_id].m_randDelay)
                    {
                        t_delayTime = UnityEngine.Random.Range(0.1f, 1f);
                    }
                    m_voiceList[_id].m_audioSource.PlayDelayed(t_delayTime);
                }
                m_currentVoices.Add(m_voiceList[_id]);
            }
            else
            {
                Debug.LogError("Didnt find voice");
                return;
            }
        }

        public void PlayVoice(int _id, Vector3 _pos)
        {
            if (_id >= 0 && _id < m_voiceList.Length)
            {
                Debug.Log("Playing voice");
                m_voiceList[_id].m_object.transform.position = _pos;
                if (m_voiceList[_id].m_delayTime == 0)
                {
                    m_voiceList[_id].m_audioSource.Play();
                }
                else if (m_voiceList[_id].m_delayTime > 0 || m_voiceList[_id].m_randDelay)
                {
                    float t_delayTime = m_voiceList[_id].m_delayTime;
                    if (m_voiceList[_id].m_randDelay)
                    {
                        t_delayTime = UnityEngine.Random.Range(0.1f, 1f);
                    }
                    m_voiceList[_id].m_audioSource.PlayDelayed(t_delayTime);
                }
                m_currentVoices.Add(m_voiceList[_id]);
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

        public void UpdateSoundVariables(AudioData.AudioType _audioType, int _id, float _volume, float _pitch, bool _looping, float _panStereo, float _spatialBlend, int _priority, float _startDelay)
        {
            switch (_audioType)
            {
                case AudioData.AudioType.MUSIC:
                    UpdateData(m_backgroundMusicList, _id, _volume, _pitch, _looping, _panStereo, _spatialBlend, _priority, _startDelay);
                    break;

                case AudioData.AudioType.SFX:
                    UpdateData(m_sfxList, _id, _volume, _pitch, _looping, _panStereo, _spatialBlend, _priority, _startDelay);
                    break;

                case AudioData.AudioType.VOICE:
                    UpdateData(m_voiceList, _id, _volume, _pitch, _looping, _panStereo, _spatialBlend, _priority, _startDelay);
                    break;
            }
        }

        public void UpdateSoundVariables(AudioData.AudioType _audioType, int _id, float _volume)
        {
            switch (_audioType)
            {
                case AudioData.AudioType.MUSIC:
                    UpdateData(m_backgroundMusicList, _id, _volume);
                    break;

                case AudioData.AudioType.SFX:
                    UpdateData(m_sfxList, _id, _volume);
                    break;

                case AudioData.AudioType.VOICE:
                    UpdateData(m_voiceList, _id, _volume);
                    break;
            }
        }

        public void UpdateSoundVariables(AudioData.AudioType _audioType, int _id, float _volume, float _pitch)
        {
            switch (_audioType)
            {
                case AudioData.AudioType.MUSIC:
                    UpdateData(m_backgroundMusicList, _id, _pitch);
                    break;

                case AudioData.AudioType.SFX:
                    UpdateData(m_sfxList, _id, _pitch);
                    break;

                case AudioData.AudioType.VOICE:
                    UpdateData(m_voiceList, _id, _pitch);
                    break;
            }
        }

        private void UpdateData(AudioData[] _array, int _id)
        {
            _array[_id].SetOriginalVolume(_array[_id].m_volume);
            _array[_id].m_volume = UpdateAudioDataVolume(_array[_id]);
            _array[_id].m_audioSource.volume = _array[_id].m_volume;
            _array[_id].m_audioSource.pitch = _array[_id].m_pitch;
            _array[_id].m_audioSource.loop = _array[_id].m_looping;
            _array[_id].m_audioSource.panStereo = _array[_id].m_panStereo;
            _array[_id].m_audioSource.spatialBlend = _array[_id].m_spatialBlend;
            _array[_id].m_audioSource.priority = _array[_id].m_priority;
        }

        private void UpdateData(AudioData[] _array, int _id, float _volume, float _pitch, bool _looping, float _panStereo, float _spatialBlend, int _priority, float _startDelay)
        {
            _array[_id].SetOriginalVolume(_volume);
            _array[_id].m_volume = UpdateAudioDataVolume(_array[_id]);
            _array[_id].m_audioSource.volume = _array[_id].m_volume;
            _array[_id].m_audioSource.pitch = _pitch;
            _array[_id].m_audioSource.loop = _looping;
            _array[_id].m_audioSource.panStereo = _panStereo;
            _array[_id].m_audioSource.spatialBlend = _spatialBlend;
            _array[_id].m_audioSource.priority = _priority;
            _array[_id].m_delayTime = _startDelay;
        }

        private void UpdateData(AudioData[] _array, int _id, float _volume)
        {
            _array[_id].SetOriginalVolume(_volume);
            _array[_id].m_volume = UpdateAudioDataVolume(_array[_id]);
            _array[_id].m_audioSource.volume = _array[_id].m_volume;
        }

        private void UpdateData(AudioData[] _array, int _id, float _volume, float _pitch)
        {
            _array[_id].SetOriginalVolume(_volume);
            _array[_id].m_volume = UpdateAudioDataVolume(_array[_id]);
            _array[_id].m_audioSource.volume = _array[_id].m_volume;
            _array[_id].m_audioSource.pitch = _pitch;
        }

        public void StopPlaying()
        {
            if (m_currentMusic.m_audioSource != null)
            {
                m_currentMusic.m_audioSource.Stop();
            }

            if (m_currentVoices.Count > 0)
            {
                for (int i = 0; i < m_currentVoices.Count; i++)
                {
                    m_currentVoices[i].m_audioSource.Stop();
                }
                m_currentVoices.Clear();
            }

            if (m_currentSFX.Count > 0)
            {
                for (int i = 0; i < m_currentSFX.Count; i++)
                {
                    m_currentSFX[i].m_audioSource.Stop();
                }
                m_currentSFX.Clear();
            }
        }

        public void PauseUnpauseAudio()
        {
            if (m_currentMusic.m_audioSource != null)
            {
                if (m_currentMusic.m_audioSource.isPlaying)
                {
                    m_currentMusic.m_audioSource.Pause();
                }
                else
                {
                    m_currentMusic.m_audioSource.UnPause();
                }
            }
            if (m_currentVoices.Count > 0)
            {
                for (int i = 0; i < m_currentVoices.Count; i++)
                {
                    if (m_currentVoices[i].m_audioSource.isPlaying)
                    {
                        m_currentVoices[i].m_audioSource.Pause();
                    }
                    else
                    {
                        m_currentVoices[i].m_audioSource.UnPause();
                    }
                }
            }

            if (m_currentSFX.Count > 0)
            {
                for (int i = 0; i < m_currentSFX.Count; i++)
                {
                    if (m_currentSFX[i].m_audioSource.isPlaying)
                    {
                        m_currentSFX[i].m_audioSource.Pause();
                    }
                    else
                    {
                        m_currentSFX[i].m_audioSource.UnPause();
                    }
                }
            }
        }
    }
}