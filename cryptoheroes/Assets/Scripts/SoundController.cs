using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioSource _musicSource;
    public static AudioSource musicSource;
    public AudioClip[] soundClips;
    public static Dictionary<string, AudioClip> soundDict = new Dictionary<string, AudioClip>();
    public static SoundController soundController;
    public GameObject _soundPrefab;
    public static GameObject soundPrefab;
    private void Awake()
    {
        soundController = this;
        soundPrefab = _soundPrefab;
        musicSource = _musicSource;
        for (int i = 0; i < soundClips.Length; i++)
        {
            soundDict.Add(soundClips[i].name, soundClips[i]);
        }
    }
    public static void PlayMusic(string musicName)
    {
        soundController.StartCoroutine(LoadMusicAndPlay(musicName));
    }
    public static void StopMusic()
    {
        soundController.StartCoroutine(StopMusicCoroutine());
    }
    public static IEnumerator StopMusicCoroutine()
    {
        float timer;
        if (musicSource.clip != null)
        {
            timer = 0;
            while (timer <= 1)
            {
                musicSource.volume = Mathf.Lerp((100 - Database.databaseStruct.musicVolume) / 100f, 0, timer);
                timer += Time.deltaTime;
            }
            musicSource.volume = 0;
            musicSource.Stop();
            musicSource.clip.UnloadAudioData();
            while (musicSource.clip.loadState != AudioDataLoadState.Unloaded)
            {
                yield return null;
            }
        }
    }
    public static IEnumerator LoadMusicAndPlay(string musicName)
    {
        float timer;
        if (musicSource.clip != null)
        {
            timer = 0;
            while (timer <= 1)
            {
                musicSource.volume = Mathf.Lerp((100 - Database.databaseStruct.musicVolume) / 100f, 0, timer);
                timer += Time.deltaTime;
            }
            musicSource.volume = 0;
            musicSource.Stop();
            musicSource.clip.UnloadAudioData();
            while (musicSource.clip.loadState != AudioDataLoadState.Unloaded)
            {
                yield return null;
            }
        }
        AudioClip audioClip = soundDict[musicName];
        audioClip.LoadAudioData();
        while (audioClip.loadState != AudioDataLoadState.Loaded)
        {
            yield return null;
        }
        musicSource.volume = 0;
        musicSource.clip = audioClip;
        musicSource.Play();
        timer = 0;
        while (timer <= 1)
        {
            musicSource.volume = Mathf.Lerp(0, (100 - Database.databaseStruct.musicVolume) / 100f, timer);
            timer += Time.deltaTime;
        }
        musicSource.volume = (100 - Database.databaseStruct.musicVolume) / 100f;
    }
    public static void OnChangeMusicVolume()
    {
        musicSource.volume = (100 - Database.databaseStruct.musicVolume) / 100f;
    }
    public static void PlaySound(string soundName, float volumeModifier = 1, bool randomPitch = false)
    {
        if (Time.timeScale >= 5)
        {
            return;
        }
        GameObject soundObj = Instantiate(soundPrefab);
        AudioSource audioSource = soundObj.GetComponent<AudioSource>();
        audioSource.clip = soundDict[soundName];
        audioSource.Play();
        audioSource.volume = (100 - Database.databaseStruct.soundVolume) / 100f * volumeModifier;
        audioSource.pitch = randomPitch ? BaseUtils.RandomFloat(.9f, 1.1f) : 1;
        soundController.StartCoroutine(WaitAndDispose(audioSource));
    }
    public static IEnumerator WaitAndDispose(AudioSource audioSource)
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        Destroy(audioSource.gameObject);
    }
}
