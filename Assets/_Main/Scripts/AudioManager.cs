
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
        soundSource.PlayOneShot(clip);
    }


    /// <summary>
    /// Play sound clip passed in in 3D space, with optional random pitch (0-1 range).
    /// Automatically creates an audio source for playback using our PoolManager.
    /// </summary>
    public void Play3D(AudioClip clip, Vector3 position, float pitch = 0f)
    {
        //cancel execution if clip wasn't set
        //if (clip == null) return;
        //calculate random pitch in the range around 1, up or down
        //pitch = Random.Range(1 - pitch, 1 + pitch);

        //activate new audio gameobject from pool
        //GameObject audioObj = PoolManager.Spawn(Instance.oneShotPrefab, position, Quaternion.identity);
        //get audio source for later use
        //AudioSource source = audioObj.GetComponent<AudioSource>();

        //assign properties, play clip
        //source.clip = clip;
        //source.pitch = pitch;
        //(clip, position);


        //deactivate audio gameobject when the clip stops playing
        //PoolManager.Despawn(audioObj, clip.length);

        var source = Instantiate(sound3DSource, position, Quaternion.identity);

        source.clip = clip;

        //source.pitch = pitch;

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
