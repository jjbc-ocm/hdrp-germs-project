
using System.Collections;
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

    private int clipIndexPlaying;

    public AudioClip[] MusicClips { get => musicClips; }

    public int ClipIndexPlaying { get => clipIndexPlaying; }

    public void PlayMusic(int index)
    {
        /*StartCoroutine(FadeOut(musicSource, 1f));

        musicSource.clip = musicClips[index];

        musicSource.Play();

        StartCoroutine(FadeIn(musicSource, 1f));*/

        StartCoroutine(FadeOutAndIn(musicSource, musicSource.clip, musicClips[index], 1f));

        clipIndexPlaying = index;
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






    #region Private

    private IEnumerator FadeIn(AudioSource audioSource, float duration)
    {
        float startTime = Time.time;
        float startVolume = 0.0f;
        float endVolume = 1.0f;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, t);
            yield return null;
        }

        audioSource.volume = endVolume;
    }

    private IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
        float startTime = Time.time;
        float startVolume = audioSource.volume;
        float endVolume = 0.0f;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, t);
            yield return null;
        }

        audioSource.volume = endVolume;
        audioSource.Stop();
    }

    private IEnumerator FadeOutAndIn(AudioSource audioSource, AudioClip clipToFadeOut, AudioClip clipToFadeIn, float duration)
    {
        // Fade out the current clip
        float startTime = Time.time;
        float startVolume = audioSource.volume;
        float endVolume = 0.0f;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, t);
            yield return null;
        }

        audioSource.volume = endVolume;
        audioSource.Stop();

        // Fade in the new clip
        audioSource.clip = clipToFadeIn;
        audioSource.Play();

        startTime = Time.time;
        startVolume = 0.0f;
        endVolume = 1.0f;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, t);
            yield return null;
        }

        audioSource.volume = endVolume;
    }

    #endregion
}
