using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public ManagementOpenCloseScene openCloseScene;
    public bool isWebGlBuild;
    public TypeDevice principalDevice;
    public TypeDevice _currentDevice;
    public event Action<TypeDevice> OnDeviceChanged;
    public TypeDevice currentDevice
    {
        get => _currentDevice;
        set
        {
            if (_currentDevice != value)
            {
                _currentDevice = value;
                OnDeviceChanged?.Invoke(_currentDevice);
            }
        }
    }
    public bool isPause;
    public bool _startGame;
    public Action<bool> OnStartGame;
    public bool startGame
    {
        get => _startGame;
        set
        {
            if (_startGame != value)
            {
                _startGame = value;
                OnStartGame?.Invoke(_startGame);
            }
        }
    }
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
        SetInitialDevice();
    }
    void LateUpdate()
    {
        CheckCurrentDevice();
    }
    public void ChangeSceneSelector(TypeScene typeScene)
    {        
        switch (typeScene)
        {
            case TypeScene.OptionsScene:
                if (!SceneManager.GetSceneByName("OptionsScene").isLoaded) SceneManager.LoadScene("OptionsScene", LoadSceneMode.Additive);
                break;
            case TypeScene.CreditsScene:
                if (!SceneManager.GetSceneByName("CreditsScene").isLoaded) SceneManager.LoadScene("CreditsScene", LoadSceneMode.Additive);
                break;
            case TypeScene.GameOverScene:
                Cursor.visible = true;
                if (!SceneManager.GetSceneByName("GameOverScene").isLoaded) SceneManager.LoadScene("GameOverScene", LoadSceneMode.Additive);
                break;
            default:
                _ = ChangeScene(typeScene);
                break;
        }
    }
    public async Awaitable ChangeScene(TypeScene typeScene)
    {
        startGame = false;
        openCloseScene.openCloseSceneAnimator.SetBool("Out", true);
        await AudioManager.Instance.FadeOut();
        while (openCloseScene.openCloseSceneAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) await Task.Delay(TimeSpan.FromSeconds(0.05)); ;
        if (typeScene == TypeScene.Reload)
        {
            ValidateActiveMouse(SceneManager.GetSceneAt(0).name);
            SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);
        }
        else if (typeScene == TypeScene.Exit)
        {
            Application.Quit();
        }
        else
        {
            ValidateActiveMouse(typeScene.ToString());
            SceneManager.LoadScene(typeScene.ToString());
        }
        await Task.Delay(TimeSpan.FromSeconds(0.05));
        _ = openCloseScene.WaitFinishCloseAnimation();
        _ = AudioManager.Instance.FadeIn();
    }
    public void ValidateActiveMouse(string typeScene)
    {
        if (typeScene == TypeScene.HomeScene.ToString() ||
            typeScene == TypeScene.CreditsScene.ToString() ||
            typeScene == TypeScene.OptionsScene.ToString())
        {
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
        }
    }
    public void SetInitialDevice()
    {
        if (!isWebGlBuild)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.WindowsPlayer ||
                 Application.platform == RuntimePlatform.OSXPlayer ||
                 Application.platform == RuntimePlatform.LinuxPlayer)
            {
                currentDevice = TypeDevice.PC;
                principalDevice = TypeDevice.PC;
            }
            else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                currentDevice = TypeDevice.MOBILE;
                principalDevice = TypeDevice.MOBILE;
            }
            else
            {
                currentDevice = TypeDevice.GAMEPAD;
                principalDevice = TypeDevice.GAMEPAD;
            }
        }
        else
        {
            currentDevice = TypeDevice.PC;
            principalDevice = TypeDevice.PC;
        }
    }
    void CheckCurrentDevice()
    {
        if (!isWebGlBuild)
        {
            if (ValidateDeviceIsMobile())
            {
                currentDevice = TypeDevice.MOBILE;
            }
            else if (IsGamepadInput())
            {
                currentDevice = TypeDevice.GAMEPAD;
            }
            else if (ValidateDeviceIsPc())
            {
                currentDevice = TypeDevice.PC;
            }
        }
        else
        {
            currentDevice = TypeDevice.PC;
        }
    }
    bool ValidateDeviceIsMobile()
    {
        return Touchscreen.current != null;
    }
    bool ValidateDeviceIsPc()
    {
        return Keyboard.current.anyKey.wasPressedThisFrame ||
            Mouse.current.leftButton.wasPressedThisFrame ||
            Mouse.current.rightButton.wasPressedThisFrame ||
            Mouse.current.scroll.ReadValue() != Vector2.zero ||
            Mouse.current.delta.ReadValue() != Vector2.zero;
    }
    bool IsGamepadInput()
    {
        Gamepad gamepad = Gamepad.current;
        if (gamepad == null) return false;

        bool currentDeviceIsGamepad = Gamepad.current != null;
        bool validateAnyGamepadInput =
            gamepad.buttonSouth.wasPressedThisFrame ||
            gamepad.buttonNorth.wasPressedThisFrame ||
            gamepad.buttonEast.wasPressedThisFrame ||
            gamepad.buttonWest.wasPressedThisFrame ||
            gamepad.leftStick.ReadValue().magnitude > 0.1f ||
            gamepad.rightStick.ReadValue().magnitude > 0.1f ||
            gamepad.dpad.ReadValue().magnitude > 0.1f ||
            gamepad.leftTrigger.wasPressedThisFrame ||
            gamepad.rightTrigger.wasPressedThisFrame;

        return currentDeviceIsGamepad && validateAnyGamepadInput;
    }
    public enum TypeScene
    {
        HomeScene = 0,
        OptionsScene = 1,
        GameScene = 2,
        CreditsScene = 3,
        Reload = 4,
        Exit = 5,
        GameOverScene = 6,

    }
    public enum TypeDevice
    {
        None,
        PC,
        GAMEPAD,
        MOBILE,
    }
}
