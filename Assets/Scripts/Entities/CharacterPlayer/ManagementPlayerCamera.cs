using Cinemachine;
using UnityEngine;

public class ManagementPlayerCamera : MonoBehaviour
{
    [SerializeField] Character character;
    [SerializeField] CinemachineVirtualCamera vcam;
    [SerializeField] float speed = 0.01f;
    public Vector3 camForward;
    public Vector3 camRight;
    public void MoveCamera()
    {
        CamDirection();
        if (character.characterInputs.characterActions.CharacterInputs.UnlockCamera.IsInProgress())
        {
            var orbital = vcam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
            orbital.m_XAxis.Value += character.characterInputs.characterActions.CharacterInputs.MoveCamera.ReadValue<Vector2>().x * speed;
        }
    }
    void CamDirection()
    {
        Vector3 camForwardDirection = Camera.main.transform.forward;
        Vector3 camRightDirection = Camera.main.transform.right;

        camForwardDirection.y = 0;
        camRightDirection.y = 0;

        camForward = camForwardDirection.normalized;
        camRight = camRightDirection.normalized;
    }
}
