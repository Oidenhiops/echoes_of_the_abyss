using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerHelper : MonoBehaviour
{
    [SerializeField] Animator _unloadAnimator;
    public void ChangeScene(int typeScene)
    {
        GameManager.TypeScene scene = (GameManager.TypeScene)typeScene;
        GameManager.Instance.ChangeSceneSelector(scene);
    }
    public void PlayASound(AudioClip audioClip)
    {
        AudioManager.Instance.PlayASound(audioClip);
    }
    public void PlayASound(AudioClip audioClip, float initialRandomPitch)
    {
        AudioManager.Instance.PlayASound(audioClip, initialRandomPitch, false);
    }
    public void PlayASoundButton(AudioClip audioClip)
    {
        AudioManager.Instance.PlayASound(audioClip, 1, false);
    }
    public void SetAudioMixerData()
    {
        AudioManager.Instance.SetAudioMixerData();
    }
    public void UnloadScene()
    {
        string sceneForUnload = ValidateScene();
        _= UnloadSceneOptions(sceneForUnload);
    }
    public string ValidateScene()
    {
        int sceneCount = SceneManager.sceneCount;
        List<string> scenes = new List<string>();
        for (int i = 0; i < sceneCount; i++)
        {
            scenes.Add(SceneManager.GetSceneAt(i).name);
        }
        if (scenes.Contains("CreditsScene")) return "CreditsScene";
        return "OptionsScene";
    }
    public async Awaitable UnloadSceneOptions(string sceneForUnload)
    {
        try
        {
            _unloadAnimator.SetBool("exit", true);
            await Task.Delay(TimeSpan.FromSeconds(0.25f));
            while (_unloadAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.05));
            }
            Scene scene = SceneManager.GetSceneByName("HomeScene");
            if (scene.IsValid() && scene.isLoaded)
            {
                MenuHelper menuHelper = FindAnyObjectByType<MenuHelper>();
                if (menuHelper != null)
                {
                    menuHelper.SelectButton();
                }
            }
            if (sceneForUnload == "OptionsScene")
            {
                Time.timeScale = 1;
                GameManager.Instance.isPause = false;
            }
            _ = SceneManager.UnloadSceneAsync(sceneForUnload);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }
    }
}
