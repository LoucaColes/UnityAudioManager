using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioManager
{
    // Main class that controls the audio
    public class AudioManager : MonoBehaviour
    {
        // Singleton of the audio manager
        public static AudioManager m_instance;

        // List of empty game objects used to store the
        // different types of audio
        [SerializeField]
        private List<GameObject> m_categories;

        // Array of audio data for background music
        public AudioData[] m_backgroundMusicList;

        // Array of audio data for sfx
        public AudioData[] m_sfxList;

        // Array of audio data for voice
        public AudioData[] m_voiceList;

        // Current background music
        [SerializeField]
        private AudioData m_currentMusic;

        // Previous background music
        [SerializeField]
        private AudioData m_prevMusic;

        // List of the current sfx playing
        [SerializeField]
        private List<AudioData> m_currentSFX;

        // List of the current voice lines playing
        [SerializeField]
        private List<AudioData> m_currentVoices;

        // Global sfx volume
        [Range(0, 1), SerializeField]
        private float m_sfxGlobalVolume = 1f;

        // Global music volume
        [Range(0, 1), SerializeField]
        private float m_musicGlobalVolume = 1f;

        // Global voice volume
        [Range(0, 1), SerializeField]
        private float m_voiceGlobalVolume = 1f;

        // Global master volume
        [Range(0, 1), SerializeField]
        private float m_masterVolume = 1f;

        // Minimum and maximum volume for random volume
        [SerializeField]
        private Vector2 m_volumeMinMax;

        // Minimum and maximum pitch for random pitch
        [SerializeField]
        private Vector2 m_pitchMinMax;

        private void Awake()
        {
            // Create the various lists
            m_categories = new List<GameObject>();
            m_currentSFX = new List<AudioData>();
            m_currentVoices = new List<AudioData>();

            // Set the object to don't destroy on load
            DontDestroyOnLoad(gameObject);

            // Create singleton of the Audio Manager
            CreateInstance();

            // Set up the audio arrays
            SetUpAudioArray(m_backgroundMusicList, "Background Music");

            SetUpAudioArray(m_sfxList, "SFX");

            SetUpAudioArray(m_voiceList, "Voice Lines");

            // Clear the current and previous music variables
            ClearCurrentPrevMusic();
        }

        // Creates singleton of the jukebox
        private void CreateInstance()
        {
            // If there is no instance set this
            // as the instance
            if (!m_instance)
            {
                m_instance = this;
            }
            // Else destroy the object with this
            // script
            else
            {
                Destroy(gameObject);
            }
        }

        // Set up the audio array
        private void SetUpAudioArray(AudioData[] _array, string _arrayName)
        {
            // Create an empty game object for the different types of sound
            GameObject t_catergory = new GameObject(_arrayName);
            t_catergory.transform.parent = transform; // Parent to audio manager
            m_categories.Add(t_catergory); // Add to list of catergories

            // For each array entry create empty game object
            for (int i = 0; i < _array.Length; i++)
            {
                // Name the object based on the sounds name
                GameObject t_child = new GameObject(_array[i].m_name);
                // Parent to the catergory game object
                t_child.transform.parent = t_catergory.transform;

                // Create a audio source for the sound object
                AudioSource t_audioSource = t_child.AddComponent<AudioSource>();

                // Transfer game object and audio source to the track's audio data
                _array[i].m_object = t_child;
                _array[i].m_audioSource = t_audioSource;

                // Transfer track's audio data's audio clip to the audio source
                _array[i].m_audioSource.clip = _array[i].m_audioClip;

                // Update the rest of the audio source data with the audio data's version
                UpdateData(_array, i);
            }
        }

        // Update an arrays volume
        private void UpdateArrayVolume(AudioData[] _array)
        {
            // Loop through each entry and update the volume based on the global volumes
            for (int i = 0; i < _array.Length; i++)
            {
                _array[i].m_volume = UpdateAudioDataVolume(_array[i]);
                _array[i].m_audioSource.volume = _array[i].m_volume;
            }
        }

        // Update volumes based on global volumes
        private float UpdateAudioDataVolume(AudioData _audioData)
        {
            // If the audio type is music then use the global music volume
            if (_audioData.m_type == AudioData.AudioType.MUSIC)
            {
                return _audioData.GetOriginalVolume() * m_musicGlobalVolume * m_masterVolume;
            }
            // If the audio type is sfx then use the global sfx volume
            else if (_audioData.m_type == AudioData.AudioType.SFX)
            {
                return _audioData.GetOriginalVolume() * m_sfxGlobalVolume * m_masterVolume;
            }
            // Else presume the audio type is volume (only 3 types at the moment)
            // and use the global voice volume
            else
            {
                return _audioData.GetOriginalVolume() * m_voiceGlobalVolume * m_masterVolume;
            }
        }

        // Set the current and previous music track to null
        private void ClearCurrentPrevMusic()
        {
            m_currentMusic = null;
            m_prevMusic = m_currentMusic;
        }

        // Play music by passing in the name of the track
        public void PlayMusic(string _name)
        {
            Debug.Log("Playing Music");
            // Check if there is an audio data based on the passed in name
            AudioData t_data = Array.Find(m_backgroundMusicList, bgm => bgm.m_name == _name);
            // If there isn't display error and return
            if (t_data == null)
            {
                Debug.LogError("Didnt find music");
                return;
            }
            // Else play new track
            else
            {
                // Set previous music to the current
                // Set current to the found audio data
                m_prevMusic = m_currentMusic;
                m_currentMusic = t_data;
                PlayNextMusicTrack(); // Play new track
            }
        }

        // Play music by passing in the name of the track and a position in world space
        public void PlayMusic(string _name, Vector3 _pos)
        {
            Debug.Log("Playing Music");
            // Check if there is an audio data based on the passed in name
            AudioData t_data = Array.Find(m_backgroundMusicList, bgm => bgm.m_name == _name);
            // If there isn't display error and return
            if (t_data == null)
            {
                Debug.LogError("Didnt find music");
                return;
            }
            // Else play new track
            else
            {
                // Set previous music to the current
                // Set current to the found audio data
                m_prevMusic = m_currentMusic;
                m_currentMusic = t_data;
                // Set found audio data's object position
                t_data.m_object.transform.position = _pos;
                PlayNextMusicTrack(); // Play new track
            }
        }

        // Play music by passing in the id of the track
        public void PlayMusic(int _id)
        {
            // Check if the passed in id is valid
            if (_id >= 0 && _id < m_backgroundMusicList.Length)
            {
                Debug.Log("Playing Music");

                // Set previous music to current music
                m_prevMusic = m_currentMusic;
                // Set current music to new track
                m_currentMusic = m_backgroundMusicList[_id];
                // Play new track
                PlayNextMusicTrack();
            }
            else // If not valid
            {
                // Display debug message and return
                Debug.LogError("Didnt find music");
                return;
            }
        }

        // Play music by passing in the name of the track and a position in world space
        public void PlayMusic(int _id, Vector3 _pos)
        {
            // Check if the passed in id is valid
            if (_id >= 0 && _id < m_backgroundMusicList.Length)
            {
                Debug.Log("Playing Music");

                // Set previous music to current music
                m_prevMusic = m_currentMusic;
                // Set current music to new track
                m_currentMusic = m_backgroundMusicList[_id];
                // Set the current music's audio data's object position
                m_currentMusic.m_object.transform.position = _pos;
                // Play new track
                PlayNextMusicTrack();
            }
            else // If not valid
            {
                // Display debug message and return
                Debug.LogError("Didnt find music");
                return;
            }
        }

        // Play the new track
        private void PlayNextMusicTrack()
        {
            // Check if there is a delay
            if (m_currentMusic.m_delayTime > 0 || m_currentMusic.m_randDelay)
            {
                // Set the delay time
                float t_delayTime = m_currentMusic.m_delayTime;
                if (m_currentMusic.m_randDelay)
                {
                    t_delayTime = UnityEngine.Random.Range(0.1f, 1f);
                }
                // Play track after delayed time
                m_currentMusic.m_audioSource.PlayDelayed(t_delayTime);
            }
            else // If no delay
            {
                // Play the new track
                m_currentMusic.m_audioSource.Play();
                // If there is no fade set
                if (!m_currentMusic.m_fade && m_prevMusic != null)
                {
                    // Stop the previous music track
                    m_prevMusic.m_audioSource.Stop();
                }
                // If there is fade set
                if (m_currentMusic.m_fade)
                {
                    // Fade both music tracks
                    StartCoroutine(FadeIn(m_currentMusic));
                    if (m_prevMusic != null)
                    {
                        StartCoroutine(FadeOut(m_prevMusic));
                    }
                }
            }
        }

        // Play sfx by passing in the sounds name
        public void PlaySFX(string _name)
        {
            // Check if that sound exists
            AudioData t_data = Array.Find(m_sfxList, sfx => sfx.m_name == _name);
            if (t_data == null) // If it doesn't display error and return
            {
                Debug.LogError("Didnt find sound");
                return;
            }
            else // If it exists play sfx
            {
                PlayFoundSFX(t_data);
            }
        }

        // Play sfx by passing in the name and a world position
        public void PlaySFX(string _name, Vector3 _pos)
        {
            // Check if that sfx exists
            AudioData t_data = Array.Find(m_sfxList, sfx => sfx.m_name == _name);
            if (t_data == null) // If it doesn't exist display error message and return
            {
                Debug.LogError("Didnt find sound");
                return;
            }
            else // If it exists then play the sfx
            {
                t_data.m_object.transform.position = _pos;
                PlayFoundSFX(t_data);
            }
        }

        // Play the found sfx audio data
        private void PlayFoundSFX(AudioData t_data)
        {
            // Check if the user wants a random volume and if true then randomise the volume
            if (t_data.m_randVolume)
            {
                t_data.m_audioSource.volume = UnityEngine.Random.Range(m_volumeMinMax.x, m_volumeMinMax.y);
            }
            // Check if the user wants a random pitch and if true then randomise the pitch
            if (t_data.m_randPitch)
            {
                t_data.m_audioSource.pitch = UnityEngine.Random.Range(m_pitchMinMax.x, m_pitchMinMax.y);
            }
            // Check if there is a delay
            // If there isn't then just play the sfx
            if (t_data.m_delayTime == 0)
            {
                t_data.m_audioSource.Play();
            }
            // If there is
            else if (t_data.m_delayTime > 0 || t_data.m_randDelay)
            {
                // Set the delay time
                float t_delayTime = t_data.m_delayTime;
                if (t_data.m_randDelay)
                {
                    t_delayTime = UnityEngine.Random.Range(0.1f, 1f);
                }
                // Play the sfx after the delayed time
                t_data.m_audioSource.PlayDelayed(t_delayTime);
            }
            // Add the sfx to the currently playing sfx list
            m_currentSFX.Add(t_data);
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
                Jukebox.m_instance.UpdateVolume();
            }
        }

        public float GetMusicGlobalVolume()
        {
            return m_musicGlobalVolume;
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
                Jukebox.m_instance.UpdateVolume();
            }
        }

        public float GetMasterVolume()
        {
            return m_masterVolume;
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
            Jukebox.m_instance.UpdateVolume();
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

        public void UpdateData(AudioData[] _array, int _id)
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