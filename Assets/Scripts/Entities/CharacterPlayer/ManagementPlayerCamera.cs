using Unity.Cinemachine;
using UnityEngine;

public class ManagementPlayerCamera : MonoBehaviour
{
    [SerializeField] Character character;
    [SerializeField] CinemachineOrbitalFollow vcam;
    [SerializeField] float speed = 0.01f;
    public void MoveCamera()
    {
        if (character.characterInputs.characterActionsInfo.isUnlockCamera)
        {
            vcam.HorizontalAxis.Value += character.characterInputs.characterActions.CharacterInputs.MoveCamera.ReadValue<Vector2>().x * speed;
            if (vcam.HorizontalAxis.Value == 0) vcam.HorizontalAxis.Value += 0.001f;
        }
    }
}
