using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterInputs : MonoBehaviour
{
    public Character character;
    public CharacterActions characterActions;
    public CharacterActionsInfo characterActionsInfo;
    public GameObject attackDirection;
    public GameObject mousePos;
    private float timeRestoreMovementMouse = 1f;
    public float restoreMovementMouse = 0;
    void OnEnable()
    {
        characterActions.Enable();
    }
    void OnDisable()
    {
        characterActions.Disable();
    }
    void Awake()
    {
        characterActions = new CharacterActions();
        InitInputs();
    }
    void Update()
    {
        CurrentDevice();
        characterActionsInfo.moveCamera = DirectionPosition();
    }
    void InitInputs()
    {
        characterActions.CharacterInputs.MousePos.performed += OnMouseInput;
        characterActions.CharacterInputs.MousePos.canceled += OnMouseInput;
        characterActions.CharacterInputs.Movement.performed += OnMovementInput;
        characterActions.CharacterInputs.Movement.canceled += OnMovementInput;
        characterActions.CharacterInputs.ActiveSkill.performed += OnActiveSkill;
        characterActions.CharacterInputs.ActiveSkill.canceled += OnActiveSkill;
        characterActions.CharacterInputs.UnlockCamera.performed += OnUnlockCamera;
        characterActions.CharacterInputs.UnlockCamera.canceled += OnUnlockCamera;
        characterActions.CharacterInputs.Pause.performed += OnPauseInput;
        characterActions.CharacterInputs.SecondaryAction.started += OnEnableSecondaryAction;        
        character.characterInputs.characterActions.CharacterInputs.ShowStats.started += OnShowStats;
    }
    void OnActiveSkill(InputAction.CallbackContext context)
    {
        if (context.action.IsPressed())
        {
            characterActionsInfo.isSkillsActive = true;
        }
        else
        {
            characterActionsInfo.isSkillsActive = false;
        }
    }
        void OnUnlockCamera(InputAction.CallbackContext context)
    {
        if (context.action.IsPressed())
        {
            characterActionsInfo.isUnlockCamera = true;
        }
        else
        {
            characterActionsInfo.isUnlockCamera = false;
        }
    }
    void OnMovementInput(InputAction.CallbackContext context)
    {
        characterActionsInfo.movement = context.ReadValue<Vector2>();
    }
    void OnPauseInput(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.startGame)
        {
            character.gameManagerHelper.ChangeScene(1);
        }
    }
    void OnMouseInput(InputAction.CallbackContext context)
    {
        characterActionsInfo.mousePos = context.ReadValue<Vector2>();
    }
    void OnEnableSecondaryAction(InputAction.CallbackContext context)
    {
        if (!characterActionsInfo.isSkillsActive)
        {
            characterActionsInfo.isSecondaryAction = !characterActionsInfo.isSecondaryAction;
            character.characterInfo.characterScripts.managementCharacterHud.ToggleSecondaryAction(characterActionsInfo.isSecondaryAction);
        }
    }
    void OnShowStats(InputAction.CallbackContext context)
    {
        characterActionsInfo.isShowStats = !characterActionsInfo.isShowStats;
        character.characterInfo.characterScripts.managementCharacterHud.ToggleShowStatistics(characterActionsInfo.isShowStats);
    }
    void CurrentDevice()
    {
        if (!GameManager.Instance.isWebGlBuild)
        {
            if (ValidateDeviceIsMobile())
            {
                GameManager.Instance.currentDevice = GameManager.TypeDevice.MOBILE;
            }
            else if (IsGamepadInput())
            {
                GameManager.Instance.currentDevice = GameManager.TypeDevice.GAMEPAD;
            }
            else if (ValidateDeviceIsPc())
            {
                GameManager.Instance.currentDevice = GameManager.TypeDevice.PC;
            }
        }
        else
        {
            GameManager.Instance.currentDevice = GameManager.TypeDevice.PC;
        }
    }
    bool ValidateDeviceIsMobile(){
        return Touchscreen.current != null;
    }
    bool ValidateDeviceIsPc(){
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
        bool validateAnyGamepadInput = gamepad.buttonSouth.wasPressedThisFrame ||
               gamepad.buttonNorth.wasPressedThisFrame ||
               gamepad.buttonEast.wasPressedThisFrame ||
               gamepad.buttonWest.wasPressedThisFrame ||
               gamepad.leftStick.ReadValue() != Vector2.zero ||
               gamepad.rightStick.ReadValue() != Vector2.zero ||
               gamepad.dpad.ReadValue() != Vector2.zero ||
               gamepad.leftTrigger.wasPressedThisFrame ||
               gamepad.rightTrigger.wasPressedThisFrame;
        return currentDeviceIsGamepad && validateAnyGamepadInput;
    }
    Vector2 DirectionPosition()
    {
        if (characterActions.CharacterInputs.UnlockCamera.IsInProgress())
        {
            return Vector2.zero;
        }
        if (characterActions.CharacterInputs.MoveCamera.ReadValue<Vector2>() != Vector2.zero || restoreMovementMouse > 0 && characterActions.CharacterInputs.BasicAttack.triggered || restoreMovementMouse > 0 && characterActions.CharacterInputs.UseSkill.triggered)
        {
            restoreMovementMouse = timeRestoreMovementMouse;
        }
        if (restoreMovementMouse > 0)
        {
            restoreMovementMouse -= Time.deltaTime;
            ValidateShowMouse(true);
            if (GameManager.Instance.currentDevice == GameManager.TypeDevice.PC)
            {
                Ray ray = Camera.main.ScreenPointToRay(character.characterInputs.characterActionsInfo.mousePos);
                if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, LayerMask.GetMask("MouseHit")))
                {
                    mousePos.transform.position = raycastHit.point;
                    Vector2 direction = new Vector2(mousePos.transform.localPosition.x, mousePos.transform.localPosition.z).normalized;
                    return direction;
                }
            }
            else if (GameManager.Instance.currentDevice == GameManager.TypeDevice.GAMEPAD || GameManager.Instance.currentDevice == GameManager.TypeDevice.MOBILE)
            {
                if (characterActions.CharacterInputs.MoveCamera.ReadValue<Vector2>() != Vector2.zero)
                {
                    return characterActions.CharacterInputs.MoveCamera.ReadValue<Vector2>();
                }
                else
                {
                    return characterActionsInfo.moveCamera;
                }
            }
        }
        else
        {
            ValidateShowMouse(false);
        }
        if (GameManager.Instance.currentDevice == GameManager.TypeDevice.GAMEPAD)
        {
            return characterActions.CharacterInputs.MoveCamera.ReadValue<Vector2>();
        }
        if (GameManager.Instance.currentDevice == GameManager.TypeDevice.MOBILE && characterActions.CharacterInputs.BasicAttack.IsPressed())
        {
            return characterActions.CharacterInputs.MoveCamera.ReadValue<Vector2>();
        }
        return Vector2.zero;
    }
    void ValidateShowMouse(bool showAttackDiection)
    {
        if (GameManager.Instance.currentDevice != GameManager.TypeDevice.PC)
        {            
            if (showAttackDiection)
            {
                attackDirection.SetActive(true);
            }
            else
            {
                attackDirection.SetActive(false);
            }
        }
    }
    [Serializable] public class CharacterActionsInfo
    {
        public Vector2 movement = Vector2.zero;
        public Vector2 moveCamera = Vector2.zero;
        public Vector2 mousePos = new Vector2();
        public bool _isSecondaryAction;
        public Action<bool> OnSecondaryActionChange;
        public bool isSecondaryAction
        {
            get => _isSecondaryAction;
            set
            {
                if (_isSecondaryAction != value)
                {
                    _isSecondaryAction = value;
                    OnSecondaryActionChange?.Invoke(_isSecondaryAction);
                }
            }
        }
        public bool isSkillsActive = false;
        public bool isShowStats = false;
        public bool isUnlockCamera = false;
    }
}