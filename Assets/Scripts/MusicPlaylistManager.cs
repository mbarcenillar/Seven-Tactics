using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlaylistManager : MonoBehaviour
{
    [Header("Configuración de Música")]
    [SerializeField] private List<AudioClip> playlist;
    [SerializeField] private AudioSource audioSource;

    private List<AudioClip> remainingTracks;
    private static MusicPlaylistManager instance;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configurar el volumen inicial
        if (!PlayerPrefs.HasKey("Volume"))
        {
            float defaultVolume = 0.3f;
            audioSource.volume = defaultVolume;
            PlayerPrefs.SetFloat("Volume", defaultVolume);
            PlayerPrefs.Save();
        }
        else
        {
            audioSource.volume = PlayerPrefs.GetFloat("Volume");
        }

        //Inicializar lista de canciones restantes
        remainingTracks = new List<AudioClip>(playlist);

        //Configurar el AudioSource
        audioSource.loop = false;
        audioSource.playOnAwake = false;

        PlayNextTrack();
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        //Si el AudioSource no está reproduciendo, pasa al siguiente clip
        if (!audioSource.isPlaying && audioSource.clip != null)
        {
            PlayNextTrack();
        }
    }

    private void PlayNextTrack()
    {
        if (remainingTracks.Count == 0)
        {
            //Si todas las canciones se han reproducido, reinicia la lista
            remainingTracks = new List<AudioClip>(playlist);
        }

        //Seleccionar una canción aleatoria de las canciones restantes
        int randomIndex = Random.Range(0, remainingTracks.Count);
        AudioClip nextTrack = remainingTracks[randomIndex];
        remainingTracks.RemoveAt(randomIndex); // Eliminarla de las canciones restantes

        //Reproducir la canción seleccionada
        audioSource.clip = nextTrack;
        audioSource.Play();
    }

    public void AddTrack(AudioClip newTrack)
    {
        if (newTrack != null && !playlist.Contains(newTrack))
        {
            playlist.Add(newTrack);
        }
    }

    public void RemoveTrack(AudioClip track)
    {
        if (track != null && playlist.Contains(track))
        {
            playlist.Remove(track);
        }
    }

    public void SkipTrack()
    {
        PlayNextTrack();
    }

    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
            PlayerPrefs.SetFloat("Volume", volume);
            PlayerPrefs.Save();
        }
    }

    public float GetVolume()
    {
        return audioSource != null ? audioSource.volume : 0f;
    }
}
