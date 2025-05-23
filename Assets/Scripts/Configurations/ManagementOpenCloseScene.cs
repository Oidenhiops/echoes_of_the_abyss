using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ManagementOpenCloseScene : MonoBehaviour
{
    public Animator openCloseSceneAnimator;
    public bool _finishLoad;
    public Action<bool>OnFinishLoadChange;
    public float speedFill;
    public bool finishLoad
    {
        get => _finishLoad;
        set
        {
            if (_finishLoad != value)
            {
                _finishLoad = value;
                OnFinishLoadChange?.Invoke(_finishLoad);
            }
        }
    }
    public float currentLoad = 0;
    public Image loaderImage;
    public bool auto = false;
    void Start()
    {
        ResetValues();
    }
    public void Update()
    {
        if (!finishLoad)
        {
            float value = currentLoad / 100 > 0 ? currentLoad / 100 : 1;
            loaderImage.fillAmount = Mathf.MoveTowards(loaderImage.fillAmount, value, speedFill * Time.unscaledDeltaTime);
            if (loaderImage.fillAmount == 1)
            {
                finishLoad = true;
                openCloseSceneAnimator.SetBool("Out", false);
                _= FinishLoad();
            }
        }
    }
    public IEnumerator AutoCharge()
    {
        while (true)
        {
            if (currentLoad >= 100)
            {
                break;
            }
            currentLoad += 20;
            yield return new WaitForSecondsRealtime(0.3f);
        }
    }
    public void AdjustLoading(float amount)
    {
        currentLoad += amount;
    }
    public async Awaitable FinishLoad()
    {
        await Task.Delay(TimeSpan.FromSeconds(0.1));
        while (openCloseSceneAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.05));
        }
        GameManager.Instance.startGame = true;
        loaderImage.fillAmount = 0;
    }
    public async Awaitable WaitFinishCloseAnimation()
    {
        while (openCloseSceneAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.05));
        }
        ResetValues();
    }
    public void ResetValues()
    {
        if (auto)
        {
            try
            {
                loaderImage.fillAmount = 0;
                currentLoad = 0;
                finishLoad = false;
                StartCoroutine(AutoCharge());
            }
            catch(Exception e)
            {
                print(e);
            }
        }
    }
}