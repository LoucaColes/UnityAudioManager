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

        // Play SFX by passing in a id
        public void PlaySFX(int _id)
        {
            // Check whether the id is within range
            if (_id >= 0 && _id < m_sfxList.Length)
            {
                Debug.Log("Playing sound");
                // Check if the volume is to be random
                if (m_sfxList[_id].m_randVolume)
                {
                    // If so randomise volume
                    m_sfxList[_id].m_audioSource.volume = UnityEngine.Random.Range(0.1f, 1);
                }
                // Check if the pitch is to be random
                if (m_sfxList[_id].m_randPitch)
                {
                    // If so randomise pitch
                    m_sfxList[_id].m_audioSource.pitch = UnityEngine.Random.Range(0.3f, 3);
                }
                // Check if there is a delay and if there isn't play sfx
                if (m_sfxList[_id].m_delayTime == 0)
                {
                    m_sfxList[_id].m_audioSource.Play();
                }
                // If there is
                else if (m_sfxList[_id].m_delayTime > 0 || m_sfxList[_id].m_randDelay)
                {
                    // Set delay time and randomise if needed
                    float t_delayTime = m_sfxList[_id].m_delayTime;
                    if (m_sfxList[_id].m_randDelay)
                    {
                        t_delayTime = UnityEngine.Random.Range(0.1f, 1f);
                    }
                    // Play the sfx after the delayed time
                    m_sfxList[_id].m_audioSource.PlayDelayed(t_delayTime);
                }
                // Add sfx to the current playing sfx list
                m_currentSFX.Add(m_sfxList[_id]);
            }
            else // If not in range display error message and return
            {
                Debug.LogError("Didnt find sound");
                return;
            }
        }

        // Play SFX by passing in a id and a world space position
        public void PlaySFX(int _id, Vector3 _pos)
        {
            // Check whether the id is within range
            if (_id >= 0 && _id < m_sfxList.Length)
            {
                Debug.Log("Playing sound");

                // Set the sound's object's position
                m_sfxList[_id].m_object.transform.position = _pos;

                // Check if the volume is to be random
                if (m_sfxList[_id].m_randVolume)
                {
                    // If so randomise volume
                    m_sfxList[_id].m_audioSource.volume = UnityEngine.Random.Range(0.1f, 1);
                }
                // Check if the pitch is to be random
                if (m_sfxList[_id].m_randPitch)
                {
                    // If so randomise pitch
                    m_sfxList[_id].m_audioSource.pitch = UnityEngine.Random.Range(0.3f, 3);
                }
                // Check if there is a delay and if there isn't play sfx
                if (m_sfxList[_id].m_delayTime == 0)
                {
                    m_sfxList[_id].m_audioSource.Play();
                }
                // If there is
                else if (m_sfxList[_id].m_delayTime > 0 || m_sfxList[_id].m_randDelay)
                {
                    // Set delay time and randomise if needed
                    float t_delayTime = m_sfxList[_id].m_delayTime;
                    if (m_sfxList[_id].m_randDelay)
                    {
                        t_delayTime = UnityEngine.Random.Range(0.1f, 1f);
                    }
                    // Play the sfx after the delayed time
                    m_sfxList[_id].m_audioSource.PlayDelayed(t_delayTime);
                }
                // Add sfx to the current playing sfx list
                m_currentSFX.Add(m_sfxList[_id]);
            }
            // If not in range display error message and return
            else
            {
                Debug.LogError("Didnt find sound");
                return;
            }
        }

        // Play voice line by passing in the name
        public void PlayVoice(string _name)
        {
            // Attempt to get the audio date for the voice line
            AudioData t_data = Array.Find(m_voiceList, voice => voice.m_name == _name);
            if (t_data == null) // If it doesn't exist then display error message
            {
                Debug.LogError("Didnt find voice");
                return;
            }
            else // If it was found
            {
                // Check if there is a delay and if not play voice clip
                if (t_data.m_delayTime == 0)
                {
                    t_data.m_audioSource.Play();
                }
                // If there is a delay
                else if (t_data.m_delayTime > 0 || t_data.m_randDelay)
                {
                    // Set the delay time
                    float t_delayTime = t_data.m_delayTime;
                    if (t_data.m_randDelay) // Randomise delay if needed
                    {
                        t_delayTime = UnityEngine.Random.Range(0.1f, 1f);
                    }
                    // Then play voice clip after the delayed time
                    t_data.m_audioSource.PlayDelayed(t_delayTime);
                }
                // Add the current voice clip to the current voices list
                m_currentVoices.Add(t_data);
            }
        }

        // Play voice line by passing in the name and a world space position
        public void PlayVoice(string _name, Vector3 _pos)
        {
            // Attempt to get the audio date for the voice line
            AudioData t_data = Array.Find(m_voiceList, voice => voice.m_name == _name);
            if (t_data == null) // If it doesn't exist then display error message
            {
                Debug.LogError("Didnt find voice");
                return;
            }
            else // If it was found
            {
                // Set sound's object position
                t_data.m_object.transform.position = _pos;
                // Check if there is a delay and if not play voice clip
                if (t_data.m_delayTime == 0)
                {
                    t_data.m_audioSource.Play();
                }
                // If there is a delay
                else if (t_data.m_delayTime > 0 || t_data.m_randDelay)
                {
                    // Set the delay time
                    float t_delayTime = t_data.m_delayTime;
                    if (t_data.m_randDelay) // Randomise delay if needed
                    {
                        t_delayTime = UnityEngine.Random.Range(0.1f, 1f);
                    }
                    // Then play voice clip after the delayed time
                    t_data.m_audioSource.PlayDelayed(t_delayTime);
                }
                // Add the current voice clip to the current voices list
                m_currentVoices.Add(t_data);
            }
        }

        // Play voice line by passing in the id
        public void PlayVoice(int _id)
        {
            // Check whether the id is within range
            if (_id >= 0 && _id < m_voiceList.Length)
            {
                Debug.Log("Playing voice");
                // Check if there is a delay and if not play voice clip
                if (m_voiceList[_id].m_delayTime == 0)
                {
                    m_voiceList[_id].m_audioSource.Play();
                }
                // If there is a delay
                else if (m_voiceList[_id].m_delayTime > 0 || m_voiceList[_id].m_randDelay)
                {
                    // Set the delay time
                    float t_delayTime = m_voiceList[_id].m_delayTime;
                    if (m_voiceList[_id].m_randDelay) // Randomise delay if needed
                    {
                        t_delayTime = UnityEngine.Random.Range(0.1f, 1f);
                    }
                    // Then play voice clip after the delayed time
                    m_voiceList[_id].m_audioSource.PlayDelayed(t_delayTime);
                }
                // Add the current voice clip to the current voices list
                m_currentVoices.Add(m_voiceList[_id]);
            }
            else // If it wasn't found display error message
            {
                Debug.LogError("Didnt find voice");
                return;
            }
        }

        // Play voice line by passing in the id and a world space position
        public void PlayVoice(int _id, Vector3 _pos)
        {
            // Check whether the id is within range
            if (_id >= 0 && _id < m_voiceList.Length)
            {
                Debug.Log("Playing voice");
                // Set the sound's object's position
                m_voiceList[_id].m_object.transform.position = _pos;
                // Check if there is a delay and if not play voice clip
                if (m_voiceList[_id].m_delayTime == 0)
                {
                    m_voiceList[_id].m_audioSource.Play();
                }
                // If there is a delay
                else if (m_voiceList[_id].m_delayTime > 0 || m_voiceList[_id].m_randDelay)
                {
                    // Set the delay time
                    float t_delayTime = m_voiceList[_id].m_delayTime;
                    if (m_voiceList[_id].m_randDelay) // Randomise delay if needed
                    {
                        t_delayTime = UnityEngine.Random.Range(0.1f, 1f);
                    }
                    // Then play voice clip after the delayed time
                    m_voiceList[_id].m_audioSource.PlayDelayed(t_delayTime);
                }
                // Add the current voice clip to the current voices list
                m_currentVoices.Add(m_voiceList[_id]);
            }
            else // If it wasn't found display error message
            {
                Debug.LogError("Didnt find voice");
                return;
            }
        }

        // Fade in a specific audio
        private IEnumerator FadeIn(AudioData _audioData)
        {
            // Set volume to 0
            _audioData.m_audioSource.volume = 0;
            float t_volume = _audioData.m_audioSource.volume;

            // While the current volume is less than normal
            while (_audioData.m_audioSource.volume < _audioData.m_volume)
            {
                // Increase volume by fade in speed
                t_volume += _audioData.m_fadeInSpeed;
                // Update volume
                _audioData.m_audioSource.volume = t_volume;
                // Wait for 0.1 seconds
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Fade out a specific audio
        private IEnumerator FadeOut(AudioData _audioData)
        {
            // Set the temp volume to the current volume
            float t_volume = _audioData.m_audioSource.volume;

            // While volume is greater than 0
            while (_audioData.m_audioSource.volume > 0)
            {
                // Decrease volume by fade out speed
                t_volume -= _audioData.m_fadeOutSpeed;
                // Update volume
                _audioData.m_audioSource.volume = t_volume;
                // Wait for 0.1 seconds
                yield return new WaitForSeconds(0.1f);
            }
            // If the volume is 0 stop the audio from playing and reset volume
            if (_audioData.m_audioSource.volume == 0)
            {
                _audioData.m_audioSource.Stop();
                _audioData.m_audioSource.volume = _audioData.m_volume;
            }
        }

        // Set the global sfx volume
        public void SetSfxGlobalVolume(float _volume)
        {
            // Check if the passed in volume is valid
            if (_volume >= 0 && _volume <= 1)
            {
                // If valid update global sfx volume
                m_sfxGlobalVolume = _volume;
                // Update all sfx audio data volumes
                UpdateArrayVolume(m_sfxList);
            }
        }

        // Set the global music volume
        public void SetMusicGlobalVolume(float _volume)
        {
            // Check if the passed in volume is valid
            if (_volume >= 0 && _volume <= 1)
            {
                // If valid update global music volume
                m_musicGlobalVolume = _volume;
                // Update all sfx audio data volumes
                UpdateArrayVolume(m_backgroundMusicList);
                // If theres a jukebox then update jukebox volume
                if (Jukebox.m_instance)
                {
                    Jukebox.m_instance.UpdateVolume();
                }
            }
        }

        // Get the global music volume
        public float GetMusicGlobalVolume()
        {
            return m_musicGlobalVolume;
        }

        // Set the global voice volume
        public void SetVoiceGlobalVolume(float _volume)
        {
            // Check if the passed in volume is valid
            if (_volume >= 0 && _volume <= 1)
            {
                // If valid update the volume
                m_voiceGlobalVolume = _volume;
                // Update all voice audio data volumes
                UpdateArrayVolume(m_voiceList);
            }
        }

        // Set the master volume
        public void SetMasterVolume(float _volume)
        {
            // Check if passed in volume is valid
            if (_volume >= 0 && _volume <= 1)
            {
                // If valid update volume
                m_masterVolume = _volume;
                // Update all audio data volumes
                UpdateArrayVolume(m_sfxList);
                UpdateArrayVolume(m_backgroundMusicList);
                UpdateArrayVolume(m_voiceList);
                // If there is a jukebox then update its volume
                if (Jukebox.m_instance)
                {
                    Jukebox.m_instance.UpdateVolume();
                }
            }
        }

        // Get the master volume
        public float GetMasterVolume()
        {
            return m_masterVolume;
        }

        // Update the manager's global volumes
        public void UpdateGlobalVolumes(float _sfxVolume, float _musicVolume, float _voiceVolume, float _masterVolume)
        {
            // Check if new sfx volume is valid and if so set the old volume to the new one
            if (_sfxVolume >= 0 && _sfxVolume <= 1)
            {
                m_sfxGlobalVolume = _sfxVolume;
            }
            // Check if new music volume is valid and if so set the old volume to the new one
            if (_musicVolume >= 0 && _musicVolume <= 1)
            {
                m_musicGlobalVolume = _musicVolume;
            }
            // Check if new voice volume is valid and if so set the old volume to the new one
            if (_voiceVolume >= 0 && _voiceVolume <= 1)
            {
                m_voiceGlobalVolume = _voiceVolume;
            }
            // Check if new master volume is valid and if so set the old volume to the new one
            if (_masterVolume >= 0 && _masterVolume <= 1)
            {
                m_masterVolume = _masterVolume;
            }

            // Update all sound volumes
            UpdateArrayVolume(m_sfxList);
            UpdateArrayVolume(m_backgroundMusicList);
            UpdateArrayVolume(m_voiceList);
            if (Jukebox.m_instance) // If there is a jukebox update volume
            {
                Jukebox.m_instance.UpdateVolume();
            }
        }

        // Update a sound variables based on the audio type
        // Will update all variables
        public void UpdateSoundVariables(AudioData.AudioType _audioType, int _id, float _volume, float _pitch, bool _looping, float _panStereo, float _spatialBlend, int _priority, float _startDelay)
        {
            switch (_audioType)
            {
                // If music use the background music list
                case AudioData.AudioType.MUSIC:
                    UpdateData(m_backgroundMusicList, _id, _volume, _pitch, _looping, _panStereo, _spatialBlend, _priority, _startDelay);
                    break;

                // If sfx use the sfx list
                case AudioData.AudioType.SFX:
                    UpdateData(m_sfxList, _id, _volume, _pitch, _looping, _panStereo, _spatialBlend, _priority, _startDelay);
                    break;

                // If voice use the voices list
                case AudioData.AudioType.VOICE:
                    UpdateData(m_voiceList, _id, _volume, _pitch, _looping, _panStereo, _spatialBlend, _priority, _startDelay);
                    break;
            }
        }

        // Update a sound variables based on the audio type
        // Will update the volume only
        public void UpdateSoundVariables(AudioData.AudioType _audioType, int _id, float _volume)
        {
            switch (_audioType)
            {
                // If music use the background music list
                case AudioData.AudioType.MUSIC:
                    UpdateData(m_backgroundMusicList, _id, _volume);
                    break;

                // If sfx use the sfx list
                case AudioData.AudioType.SFX:
                    UpdateData(m_sfxList, _id, _volume);
                    break;

                // If voice use the voices list
                case AudioData.AudioType.VOICE:
                    UpdateData(m_voiceList, _id, _volume);
                    break;
            }
        }

        // Update a sound variables based on the audio type
        // Will update the volume and pitch
        public void UpdateSoundVariables(AudioData.AudioType _audioType, int _id, float _volume, float _pitch)
        {
            switch (_audioType)
            {
                // If music use the background music list
                case AudioData.AudioType.MUSIC:
                    UpdateData(m_backgroundMusicList, _id, _pitch);
                    break;

                // If sfx use the sfx list
                case AudioData.AudioType.SFX:
                    UpdateData(m_sfxList, _id, _pitch);
                    break;

                // If voice use the voices list
                case AudioData.AudioType.VOICE:
                    UpdateData(m_voiceList, _id, _pitch);
                    break;
            }
        }

        // Update audio source data using data set up through the inspector
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

        // Update audio source data using data thats passed in
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

        // Update the volume of a specific sound
        private void UpdateData(AudioData[] _array, int _id, float _volume)
        {
            _array[_id].SetOriginalVolume(_volume);
            _array[_id].m_volume = UpdateAudioDataVolume(_array[_id]);
            _array[_id].m_audioSource.volume = _array[_id].m_volume;
        }

        // Update the volume and pitch of a specific sound
        private void UpdateData(AudioData[] _array, int _id, float _volume, float _pitch)
        {
            _array[_id].SetOriginalVolume(_volume);
            _array[_id].m_volume = UpdateAudioDataVolume(_array[_id]);
            _array[_id].m_audioSource.volume = _array[_id].m_volume;
            _array[_id].m_audioSource.pitch = _pitch;
        }

        // Stop playing audio
        public void StopPlaying()
        {
            // Music
            // If there is an audio source for the current track stop audio
            if (m_currentMusic.m_audioSource != null)
            {
                m_currentMusic.m_audioSource.Stop();
            }

            // Voices
            // If there are any voice tracks in the current voices list
            if (m_currentVoices.Count > 0)
            {
                // Loop through the list
                for (int i = 0; i < m_currentVoices.Count; i++)
                {
                    // Stop the audio
                    m_currentVoices[i].m_audioSource.Stop();
                }
                // Clear the current voices list
                m_currentVoices.Clear();
            }

            // SFX
            // If there are any sfx tracks in the current sfx list
            if (m_currentSFX.Count > 0)
            {
                // Loop through the list
                for (int i = 0; i < m_currentSFX.Count; i++)
                {
                    // Stop the audio
                    m_currentSFX[i].m_audioSource.Stop();
                }
                // Clear the current sfx list
                m_currentSFX.Clear();
            }
        }

        // Pause/Unpause Audio
        public void PauseUnpauseAudio()
        {
            // Background Music
            // If theres an audio source for the current tracl
            if (m_currentMusic.m_audioSource != null)
            {
                // If audio is playing then pause
                if (m_currentMusic.m_audioSource.isPlaying)
                {
                    m_currentMusic.m_audioSource.Pause();
                }
                // Else unpause audio
                else
                {
                    m_currentMusic.m_audioSource.UnPause();
                }
            }
            // Voices
            // If there are any voices within the current voices list
            if (m_currentVoices.Count > 0)
            {
                // Loop through list
                for (int i = 0; i < m_currentVoices.Count; i++)
                {
                    // If the audio is playing then pause it
                    if (m_currentVoices[i].m_audioSource.isPlaying)
                    {
                        m_currentVoices[i].m_audioSource.Pause();
                    }
                    // Else unpause the audio
                    else
                    {
                        m_currentVoices[i].m_audioSource.UnPause();
                    }
                }
            }

            // SFX
            // If there are any sfx within the current voices list
            if (m_currentSFX.Count > 0)
            {
                // Loop through the list
                for (int i = 0; i < m_currentSFX.Count; i++)
                {
                    // If there is audio playing then pause it
                    if (m_currentSFX[i].m_audioSource.isPlaying)
                    {
                        m_currentSFX[i].m_audioSource.Pause();
                    }
                    // Else Unpause it
                    else
                    {
                        m_currentSFX[i].m_audioSource.UnPause();
                    }
                }
            }
        }
    }
}