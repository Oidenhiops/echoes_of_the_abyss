using Unity.Cinemachine;
using UnityEngine;

public class ManagementPlayerCamera : MonoBehaviour
{
    [SerializeField] Character character;
    [SerializeField] CinemachineOrbitalFollow vcam;
    [SerializeField] float baseSpeed = 0.01f;
    [SerializeField] float currentSpeed;
    public void Start()
    {
        GameManager.Instance.OnDeviceChanged += ChangeSpeedCamera;
    }
    public void OnDestroy()
    {
        GameManager.Instance.OnDeviceChanged -= ChangeSpeedCamera;
    }
    void ChangeSpeedCamera(GameManager.TypeDevice typeDevice)
    {
        currentSpeed = typeDevice == GameManager.TypeDevice.PC ? baseSpeed : baseSpeed * 40;
    }
    public void MoveCamera()
    {
        if (character.characterInputs.characterActionsInfo.isUnlockCamera)
        {
            vcam.HorizontalAxis.Value += character.characterInputs.characterActions.CharacterInputs.MoveCamera.ReadValue<Vector2>().x * currentSpeed;
            if (vcam.HorizontalAxis.Value == 0) vcam.HorizontalAxis.Value += 0.001f;
        }
    }
}
