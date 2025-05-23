using System;
using System.Threading.Tasks;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public AudioMixer audioMixer;
    public SoundsDBSO soundsDB;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlayASound(AudioClip audioClip)
    {
        AudioSource audioBox = Instantiate(Resources.Load<GameObject>("Prefabs/AudioBox/AudioBox")).GetComponent<AudioSource>();
        audioBox.clip = audioClip;
        audioBox.Play();
        Destroy(audioBox.gameObject, audioBox.clip.length);
    }
    public void PlayASound(AudioClip audioClip, float initialPitch, bool randomPitch)
    {
        AudioSource audioBox = Instantiate(Resources.Load<GameObject>("Prefabs/AudioBox/AudioBox")).GetComponent<AudioSource>();
        audioBox.clip = audioClip;
        audioBox.pitch = randomPitch ? UnityEngine.Random.Range(0.5f, 1.5f) : UnityEngine.Random.Range(initialPitch - 0.1f, initialPitch + 0.1f);
        audioBox.Play();
        Destroy(audioBox.gameObject, audioClip.length);
    }
    public async Awaitable FadeIn()
    {
        float targetDecibels = 20 * Mathf.Log10(GameData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue / 100);
        float currentVolume;

        if (!audioMixer.GetFloat(TypeSound.Master.ToString(), out currentVolume))
        {
            currentVolume = -80f;
        }
        float duration = 1f;
        float elapsed = 0f;
        float updateRate = 1f / 60f;
        while (elapsed < duration)
        {
            if (GameData.Instance.saveData.configurationsInfo.soundConfiguration.isMute)
                break;

            elapsed += updateRate;
            float t = Mathf.Clamp01(elapsed / duration);
            float newVolume = Mathf.Lerp(currentVolume, targetDecibels, t);
            audioMixer.SetFloat(TypeSound.Master.ToString(), newVolume);

            await Task.Delay(TimeSpan.FromSeconds(updateRate));
        }
        audioMixer.SetFloat(TypeSound.Master.ToString(), targetDecibels);
    }

    public AudioClip GetAudioClip(string typeSound)
    {
        if (soundsDB.sounds.TryGetValue(typeSound, out AudioClip[] clips))
        {
            return clips[UnityEngine.Random.Range(0, clips.Length - 1)];
        }
        return null;
    }

    public async Awaitable FadeOut()
    {
        float currentVolume;
        if (!audioMixer.GetFloat(TypeSound.Master.ToString(), out currentVolume))
        {
            currentVolume = 20 * Mathf.Log10(GameData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue / 100);
        }
        float targetVolume = -80f;
        float duration = 1f;
        float elapsed = 0f;
        float updateRate = 1f / 60f;
        while (elapsed < duration)
        {
            if (GameData.Instance.saveData.configurationsInfo.soundConfiguration.isMute)
                break;

            elapsed += updateRate;
            float t = Mathf.Clamp01(elapsed / duration);
            float newVolume = Mathf.Lerp(currentVolume, targetVolume, t);
            audioMixer.SetFloat(TypeSound.Master.ToString(), newVolume);

            await Task.Delay(TimeSpan.FromSeconds(updateRate));
        }
        audioMixer.SetFloat(TypeSound.Master.ToString(), targetVolume);
    }

    public void SetAudioMixerData()
    {
        float decibelsMaster = 20 * Mathf.Log10(GameData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue / 100);
        float decibelsBGM = 20 * Mathf.Log10(GameData.Instance.saveData.configurationsInfo.soundConfiguration.BGMalue / 100);
        float decibelsSFX = 20 * Mathf.Log10(GameData.Instance.saveData.configurationsInfo.soundConfiguration.SFXalue / 100);
        if (GameData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue == 0) decibelsMaster = -80;
        if (GameData.Instance.saveData.configurationsInfo.soundConfiguration.BGMalue == 0) decibelsBGM = -80;
        if (GameData.Instance.saveData.configurationsInfo.soundConfiguration.SFXalue == 0) decibelsSFX = -80;
        audioMixer.SetFloat(TypeSound.BGM.ToString(), decibelsBGM);
        audioMixer.SetFloat(TypeSound.SFX.ToString(), decibelsSFX);
        audioMixer.SetFloat(TypeSound.Master.ToString(), GameData.Instance.saveData.configurationsInfo.soundConfiguration.isMute ? -80 : decibelsMaster);
        GameData.Instance.SaveGameData();
    }
    public enum TypeSound
    {
        Master = 0,
        BGM = 1,
        SFX = 2
    }    
}
