
using TanksMP;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField]
    private AudioSource musicSource;

    [SerializeField]
    private AudioSource soundSource;

    [SerializeField]
    private AudioSource sound3DSource;

    [SerializeField]
    private AudioClip[] musicClips;

    public AudioClip[] MusicClips { get => musicClips; }


    // Stop playing music after switching scenes. To keep playing
    // music in the new scene, this requires calling PlayMusic() again.
    /*void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        musicSource.Stop();
    }*/


    public void PlayMusic(int index)
    {
        musicSource.clip = musicClips[index];

        musicSource.Play();
    }

    public void Play2D(AudioClip clip)
    {
        Debug.Log("[Play2D]");

        soundSource.PlayOneShot(clip);
    }


    public void Play3D(AudioClip clip, Vector3 position)
    {
        var source = Instantiate(sound3DSource, position, Quaternion.identity);

        source.clip = clip;

        //source.PlayOneShot(clip);

        source.Play();

        Destroy(source.gameObject, clip.length + 3);
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("volumeMusic", value);
    }

    public void SetSoundVolume(float value)
    {
        audioMixer.SetFloat("volumeSound", value);
    }
}
