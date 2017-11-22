using UnityEngine;

namespace AudioManager
{
    // Jukebox class where it has a queue of music tracks
    [System.Serializable]
    public class Jukebox : MonoBehaviour
    {
        // Array of the tracks to play
        public AudioData[] m_playlist;

        // Id of the current track
        [SerializeField]
        private int m_id;

        // Current track playing
        [SerializeField]
        private AudioData m_currentlyPlaying;

        // Possible states of the jukebox
        public enum JukeboxState
        {
            SetUp,
            Play,
            Pause,
            Stop,
        }

        // Current state of the jukebox
        private JukeboxState m_state;

        // Types of play
        public enum JukeboxPlayType
        {
            Random,
            Sequential,
        }

        // Current play type of the jukebox
        public JukeboxPlayType m_playType;

        // Length of the current track
        [SerializeField]
        private float m_playTime;

        // Timer to track the current point in the track
        [SerializeField]
        private float m_timer;

        // Singleton of the Jukebox
        public static Jukebox m_instance;

        // Start Function
        // Creates singleton of the jukebox
        // Sets up the jukebox state
        // Sets up the jukebox
        private void Start()
        {
            CreateInstance();
            m_state = JukeboxState.SetUp;
            SetUp();
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

        // Automatic track changing
        private void Update()
        {
            // If the jukebox is in the play state
            if (m_state == JukeboxState.Play)
            {
                // Increase the timer
                m_timer += Time.deltaTime;
                if (m_timer >= m_playTime)
                {
                    // If the timer is greater or equal to the play time
                    // of the track, play the next one
                    switch (m_playType)
                    {
                        // If play type is sequential play the next one
                        case JukeboxPlayType.Sequential:
                            PlayNext();
                            break;

                        // If play type is random play a random track
                        case JukeboxPlayType.Random:
                            PlayRandom();
                            break;
                    }
                }
            }
        }

        // Set up the jukebox
        private void SetUp()
        {
            // Set up the playlist
            SetUpAudioArray(m_playlist, "Playlist");
            m_id = 0; // Set id
            // Set the current playing track
            m_currentlyPlaying = m_playlist[m_id];

            // Reset the timer
            m_timer = 0;

            // Set the play time of the current track
            m_playTime = m_currentlyPlaying.m_audioClip.length;

            // Play the current track
            PlayCurrent();

            // Set the state to play
            m_state = JukeboxState.Play;
        }

        // Set up the playlist array (similar to the Audio Manager version)
        private void SetUpAudioArray(AudioData[] _array, string _arrayName)
        {
            // Create a empty game object to store the track objects
            GameObject t_catergory = new GameObject(_arrayName);
            t_catergory.transform.parent = transform;

            // Create a empty game object for each track
            for (int i = 0; i < _array.Length; i++)
            {
                GameObject t_child = new GameObject(_array[i].m_name);
                t_child.transform.parent = t_catergory.transform;

                // Add an audio source to the empty game object
                AudioSource t_audioSource = t_child.AddComponent<AudioSource>();

                // Transfer game object and audio source to the track's audio data
                _array[i].m_object = t_child;
                _array[i].m_audioSource = t_audioSource;

                // Transfer track's audio data's audio clip to the audio source
                _array[i].m_audioSource.clip = _array[i].m_audioClip;

                // Update the rest of the audio source data with the audio data's version
                AudioManager.m_instance.UpdateData(_array, i);
            }
        }

        // Pause/Unpause Jukebox
        public void PauseJukebox()
        {
            // If the track is playing the pause
            if (m_currentlyPlaying.m_audioSource.isPlaying)
            {
                // pause the current track
                m_currentlyPlaying.m_audioSource.Pause();
                m_state = JukeboxState.Pause; // Set the jukebox state to pause
            }
            // If the track isn't playing then unpause the track
            else
            {
                // Update the volume just in case there was a volume change
                UpdateVolume();
                // Unpause the current track
                m_currentlyPlaying.m_audioSource.UnPause();
                // Set the jukebox state to play
                m_state = JukeboxState.Play;
            }
        }

        // Stop the jukebox
        public void StopJukebox()
        {
            // Stop the current track
            m_currentlyPlaying.m_audioSource.Stop();
            // Set state to stop
            m_state = JukeboxState.Stop;
        }

        // Play Jukebox if paused or stop
        public void PlayJukebox()
        {
            switch (m_state)
            {
                // If the jukebox was in the stop state
                case JukeboxState.Stop:
                    m_timer = 0; // Reset timer
                    // Set play time
                    m_playTime = m_currentlyPlaying.m_audioClip.length;

                    // Play the current track
                    PlayCurrent();
                    // Set the state to play
                    m_state = JukeboxState.Play;
                    break;

                // If the jukebox was in the pause state
                case JukeboxState.Pause:
                    // Unpause the track
                    PauseJukebox();
                    break;
            }
        }

        // Play Next Track
        public void PlayNext()
        {
            // Stop the current track
            m_currentlyPlaying.m_audioSource.Stop();

            m_id++; // Increment the id value

            // If the id reaches the limit reset to 0
            if (m_id >= m_playlist.Length)
            {
                m_id = 0;
            }

            // Update the current track variable
            m_currentlyPlaying = m_playlist[m_id];

            // Reset timer
            m_timer = 0;

            // Update the playtime variable
            m_playTime = m_currentlyPlaying.m_audioClip.length;

            // Play the new track
            PlayCurrent();
        }

        // Play Random Track
        public void PlayRandom()
        {
            // Generate a random int until different to current id
            int t_int = m_id;
            do
            {
                t_int = Random.Range(0, m_playlist.Length);
            } while (t_int == m_id);
            m_id = t_int; // Set current id to new id

            // Stop the current track
            m_currentlyPlaying.m_audioSource.Stop();

            // Set the new current track
            m_currentlyPlaying = m_playlist[m_id];

            // Reset timer
            m_timer = 0;

            // Set the play time
            m_playTime = m_currentlyPlaying.m_audioClip.length;

            // Play the new track
            PlayCurrent();
        }

        // Play the current track
        private void PlayCurrent()
        {
            // Update the volume in case there was a volume change
            UpdateVolume();

            // Play current track
            m_currentlyPlaying.m_audioSource.Play();
        }

        // Update volume of current track
        public void UpdateVolume()
        {
            // Calculate the new volume and set the volume of current track
            m_currentlyPlaying.m_audioSource.volume = m_currentlyPlaying.GetOriginalVolume() *
                AudioManager.m_instance.GetMusicGlobalVolume() *
                AudioManager.m_instance.GetMasterVolume();
        }
    }
}