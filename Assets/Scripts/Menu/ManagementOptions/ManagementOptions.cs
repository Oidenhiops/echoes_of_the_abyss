using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManagementOptions : MonoBehaviour
{
    public TMP_Dropdown dropdownLanguage;
    public TMP_Dropdown dropdownResolution;
    public SoundInfo[] soundInfo;
    public WindowModeButtonsInfo windowModeButtonsInfo;
    public FpsButtonsInfo fpsButtonsInfo;
    public GameObject homeButton;
    public GameObject muteCheck;
    public GameObject resolutionButtons;
    public ControlsInfo[] controlsInfo;

    void OnEnable()
    {
        InitializeLanguageDropdown();
        InitializeResolutionDropdown();
        SetFullScreenButtonsSprite();
        SetFpsLimitButtonsSprite();
        SetVolumeSliders();
        GameManager.Instance.OnDeviceChanged += ChangeMenuButtons;
        ChangeMenuButtons(GameManager.Instance.currentDevice);
        muteCheck.SetActive(GameData.Instance.saveData.configurationsInfo.soundConfiguration.isMute);
        if (SceneManager.GetSceneByName("HomeScene").isLoaded) homeButton.SetActive(false);
    }
    void OnDestroy()
    {
        if (GameManager.Instance.principalDevice == GameManager.TypeDevice.PC) GameManager.Instance.OnDeviceChanged -= ChangeMenuButtons;
    }
    public void InitializeLanguageDropdown()
    {
        dropdownLanguage.options.Clear();
        foreach (GameData.TypeLanguage language in Enum.GetValues(typeof(GameData.TypeLanguage)))
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData
            {
                text = language.ToString()
            };
            dropdownLanguage.options.Add(option);
        }
        for (int i = 0; i < dropdownLanguage.options.Count; i++)
        {
            if (dropdownLanguage.options[i].text == GameData.Instance.saveData.configurationsInfo.currentLanguage.ToString())
            {
                dropdownLanguage.value = i;
                break;
            }
        }
    }
    public void ChangeLanguage()
    {
        GameData.TypeLanguage language = (GameData.TypeLanguage)dropdownLanguage.value;
        GameData.Instance.ChangeLanguage(language);
    }
    public void InitializeResolutionDropdown()
    {
        dropdownResolution.options.Clear();
        foreach (var resolutions in GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.allResolutions)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData
            {
                text = $"{resolutions.width}X{resolutions.height}"
            };
            dropdownResolution.options.Add(option);
        }
        for (int i = 0; i < dropdownResolution.options.Count; i++)
        {
            GameData.ResolutionsInfo resolutionsInfo = GetCurrentResolution(dropdownResolution.options[i].text);
            if (resolutionsInfo.width == GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.currentResolution.width &&
                resolutionsInfo.height == GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.currentResolution.height)
            {
                dropdownResolution.value = i;
                break;
            }
        }
    }
    public void SetVolumeSliders()
    {
        FindSoundInfo(AudioManager.TypeSound.Master).slider.value = GameData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue;
        FindSoundInfo(AudioManager.TypeSound.BGM).slider.value = GameData.Instance.saveData.configurationsInfo.soundConfiguration.BGMalue;
        FindSoundInfo(AudioManager.TypeSound.SFX).slider.value = GameData.Instance.saveData.configurationsInfo.soundConfiguration.SFXalue;
    }
    public SoundInfo FindSoundInfo(AudioManager.TypeSound typeSound)
    {
        foreach (var sound in soundInfo)
        {
            if (sound.typeSound == typeSound)
            {
                return sound;
            }
        }
        return null;
    }
    public SoundInfo FindSoundInfo(Slider slider)
    {
        foreach (var sound in soundInfo)
        {
            if (sound.slider == slider)
            {
                return sound;
            }
        }
        return null;
    }
    public void SetMixerValues()
    {
        AudioManager.Instance.SetAudioMixerData();
    }
    public void SetSoundValue(AudioManager.TypeSound typeSound, bool isAdd)
    {
        int amount = isAdd ? 1 : -1;
        SoundInfo soundInfo = FindSoundInfo(typeSound);
        switch (soundInfo.typeSound)
        {
            case AudioManager.TypeSound.Master:
                soundInfo.slider.value += amount;
                GameData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue = soundInfo.slider.value;
                break;
            case AudioManager.TypeSound.BGM:
                soundInfo.slider.value += amount;
                GameData.Instance.saveData.configurationsInfo.soundConfiguration.BGMalue = soundInfo.slider.value;
                break;
            case AudioManager.TypeSound.SFX:
                soundInfo.slider.value += amount;
                GameData.Instance.saveData.configurationsInfo.soundConfiguration.SFXalue = soundInfo.slider.value;
                break;
        }
        SetVolumeSliders();
        SetMixerValues();
    }
    public void SetSoundValue(Slider slider)
    {
        switch (FindSoundInfo(slider).typeSound)
        {
            case AudioManager.TypeSound.Master:
                GameData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue = slider.value;
                break;
            case AudioManager.TypeSound.BGM:
                GameData.Instance.saveData.configurationsInfo.soundConfiguration.BGMalue = slider.value;
                break;
            case AudioManager.TypeSound.SFX:
                GameData.Instance.saveData.configurationsInfo.soundConfiguration.SFXalue = slider.value;
                break;
        }
        SetVolumeSliders();
        SetMixerValues();
    }
    public void PlusVolume(int typeSound)
    {
        SetSoundValue((AudioManager.TypeSound)typeSound, true);
    }
    public void MiunsVolume(int typeSound)
    {
        SetSoundValue((AudioManager.TypeSound)typeSound, false);
    }
    public void SetMute()
    {
        GameData.Instance.saveData.configurationsInfo.soundConfiguration.isMute = !GameData.Instance.saveData.configurationsInfo.soundConfiguration.isMute;
        muteCheck.SetActive(GameData.Instance.saveData.configurationsInfo.soundConfiguration.isMute);
        SetMixerValues();
        GameData.Instance.SaveGameData();
    }
    public void SetFullScreen(bool isFullScreen)
    {
        GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.isFullScreen = isFullScreen;
        SetFullScreenButtonsSprite();
        Screen.SetResolution(
            GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.currentResolution.width,
            GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.currentResolution.height,
            GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.isFullScreen);
        GameData.Instance.SaveGameData();
    }
    public void SetFullScreenButtonsSprite()
    {
        bool isFullScreen = GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.isFullScreen;
        if (isFullScreen)
        {
            windowModeButtonsInfo.buttonsImage[0].sprite = windowModeButtonsInfo.spriteOn;
            windowModeButtonsInfo.buttonsImage[1].sprite = windowModeButtonsInfo.spriteOff;
            return;
        }
        windowModeButtonsInfo.buttonsImage[0].sprite = windowModeButtonsInfo.spriteOff;
        windowModeButtonsInfo.buttonsImage[1].sprite = windowModeButtonsInfo.spriteOn;
    }
    public void SetFpsLimit(int id)
    {
        int fps = id * 30;
        GameData.Instance.saveData.configurationsInfo.FpsLimit = fps;
        Application.targetFrameRate = fps;
        SetFpsLimitButtonsSprite();
        GameData.Instance.SaveGameData();
    }
    public void SetFpsLimitButtonsSprite()
    {
        foreach (FpsButton fpsButton in fpsButtonsInfo.buttons)
        {
            bool isSelected = fpsButton.id == GameData.Instance.saveData.configurationsInfo.FpsLimit;
            fpsButton.buttonImage.sprite = isSelected ? fpsButtonsInfo.spriteOn : fpsButtonsInfo.spriteOff;
        }
    }
    public void ChangeResolution()
    {
        GameData.ResolutionsInfo currentResolution = GetCurrentResolution(dropdownResolution.options[dropdownResolution.value].text);
        GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.currentResolution = currentResolution;
        Screen.SetResolution(
            currentResolution.width,
            currentResolution.height,
            GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.isFullScreen);
        GameData.Instance.SaveGameData();
    }
    public GameData.ResolutionsInfo GetCurrentResolution(string resolution)
    {
        int index = resolution.IndexOf("X");
        int width = int.Parse(resolution.Substring(0, index));
        int height = int.Parse(resolution.ToString().Substring(index + 1));
        return new GameData.ResolutionsInfo(width, height);
    }
    public void ChangeMenuButtons(GameManager.TypeDevice typeDevice)
    {
        foreach (ControlsInfo control in controlsInfo)
        {
            control.container.SetActive(control.typeDevice == typeDevice);
        }
        if (typeDevice != GameManager.TypeDevice.MOBILE)
        {
            resolutionButtons.SetActive(true);
        }
        else
        {
            resolutionButtons.SetActive(false);
        }
    }
    [Serializable] public class SoundInfo
    {
        public AudioManager.TypeSound typeSound;
        public Slider slider;
    }
    [Serializable] public class WindowModeButtonsInfo
    {
        public Sprite spriteOn;
        public Sprite spriteOff;
        public Image[] buttonsImage;
    }
    [Serializable] public class FpsButtonsInfo
    {
        public Sprite spriteOn;
        public Sprite spriteOff;
        public FpsButton[] buttons;
    }
    [Serializable] public class FpsButton
    {
        public int id;
        public Image buttonImage;
    }
    [Serializable] public class ControlsInfo
    {
        public GameManager.TypeDevice typeDevice;
        public GameObject container;
    }
}