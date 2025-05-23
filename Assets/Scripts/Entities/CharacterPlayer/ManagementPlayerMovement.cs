using System;
using UnityEngine;

public class ManagementPlayerMovement : MonoBehaviour, Character.ICharacterMove
{
    public Character character;
    Vector3 camForward;
    Vector3 camRight;
    Vector3 movementDirection;
    float jumpForce = 3;
    public void Move()
    {
        Vector3 inputs = new Vector3
        (
            character.characterInputs.characterActionsInfo.movement.x,
            0,
            character.characterInputs.characterActionsInfo.movement.y
        ).normalized;
        CamDirection();
        Vector3 camDirection = (inputs.x * camRight + inputs.z * camForward).normalized;
        movementDirection = new Vector3
        (
            camDirection.x,
            0,
            camDirection.z
        );
        if (!character.characterInfo.characterScripts.managementStatusEffect.statusEffects.ContainsKey(StatusEffectSO.TypeStatusEffect.Push))
        {
            Jump();
            float speed = character.characterInfo.GetStatisticByType(Character.TypeStatistics.Spd).currentValue;
            movementDirection.x *= speed;
            movementDirection.z *= speed;
            movementDirection.y = character.characterInfo.rb.linearVelocity.y;
            character.characterInfo.rb.linearVelocity = movementDirection;
        }
        else
        {
            character.characterInfo.rb.linearVelocity += movementDirection / 10;
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
    void Jump()
    {
        if (character.characterInfo.isGrounded && character.characterInputs.characterActions.CharacterInputs.Jump.triggered)
        {
            character.characterInfo.rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    public Rigidbody GetRigidbody()
    {
        return character.characterInfo.rb;
    }
    public void SetPositionTarget(Transform position){}
    public void SetCanMoveState(bool state){}

    public void SetTarget(Transform targetPos){}
}
