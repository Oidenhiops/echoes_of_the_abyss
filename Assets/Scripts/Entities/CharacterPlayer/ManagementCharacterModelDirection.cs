using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagementCharacterModelDirection : MonoBehaviour, ManagementCharacterModelDirection.ICharacterDirection
{
    public Character character;
    [SerializeField] float rayDistanceTarget = 10f;
    [SerializeField] LayerMask targetMask;
    [SerializeField] Character characterTarget;
    public Vector2 movementDirectionAnimation = new Vector2();
    public Vector2 movementCharacter = new Vector2();
    public GameObject directionPlayer;
    public Vector3 targetViewportPos;
    public Vector3 selfViewportPos;
    void Start()
    {
        rayDistanceTarget = character.characterInfo.isPlayer ? 10 : character.characterInfo.characterScripts.characterAttack.GetDistLostTarget();
    }
    public void ChangeModelDirection()
    {
        if (characterTarget != null)
        {
            LookToTarget();
        }
        else
        {
            if (character.characterInfo.isPlayer && character.characterInputs != null)
            {
                movementCharacter = character.characterInputs.characterActionsInfo.movement;
                if (character.characterInputs.characterActions.CharacterInputs.LookEnemy.triggered)
                {
                    ValidateLookToTarget();
                }
                if (GameManager.Instance.currentDevice != GameManager.TypeDevice.PC)
                {
                    if (character.characterInputs.characterActionsInfo.moveCamera == Vector2.zero)
                    {
                        MoveWhitOutCamera();
                    }
                    else
                    {
                        MoveWhitJoystick();
                    }
                }
                else
                {
                    if (character.characterInputs.characterActionsInfo.moveCamera == Vector2.zero)
                    {
                        MoveWhitOutCamera();
                    }
                    else
                    {
                        MoveWhitCameraPc();
                    }
                }
            }
        }
    }

    public void ValidateLookToTarget()
    {
        if (Physics.BoxCast(directionPlayer.transform.position, Vector3.one, directionPlayer.transform.forward, out RaycastHit objectHit, Quaternion.identity, rayDistanceTarget, targetMask))
        {
            characterTarget = objectHit.collider.GetComponent<Character>();
        }
    }

    private void LookToTarget()
    {
        targetViewportPos = Camera.main.WorldToViewportPoint(characterTarget.transform.position);
        selfViewportPos = Camera.main.WorldToViewportPoint(transform.position);
        if (targetViewportPos.x > selfViewportPos.x)
        {
            movementDirectionAnimation.x = -1;
            character.characterInfo.characterScripts.characterAnimations.GetCharacterSprite().transform.localRotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            movementDirectionAnimation.x = 1;
            character.characterInfo.characterScripts.characterAnimations.GetCharacterSprite().transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        if (targetViewportPos.z < selfViewportPos.z)
        {
            movementDirectionAnimation.y = -1;
        }
        else
        {
            movementDirectionAnimation.y = 1;
        }
        directionPlayer.transform.LookAt(new Vector3(characterTarget.transform.position.x, directionPlayer.transform.position.y, characterTarget.transform.position.z));

        if (!characterTarget.characterInfo.isActive || 
        Vector3.Distance(characterTarget.transform.position, transform.position) > rayDistanceTarget || 
        character.characterInfo.isPlayer && character.characterInputs.characterActions.CharacterInputs.LookEnemy.triggered)
        {
            characterTarget = null;
        }
    }

    void MoveWhitOutCamera()
    {
        if (character.characterInputs.characterActionsInfo.movement != Vector2.zero)
        {
            if (character.characterInputs.characterActionsInfo.movement.x != 0)
            {
                movementDirectionAnimation.x = character.characterInputs.characterActionsInfo.movement.x;
            }
            if (character.characterInputs.characterActionsInfo.movement.y != 0)
            {
                movementDirectionAnimation.y = character.characterInputs.characterActionsInfo.movement.y;
            }
            if (movementDirectionAnimation.x > 0)
            {
                character.characterInfo.characterScripts.characterAnimations.GetCharacterSprite().transform.localRotation = Quaternion.Euler(0, -180, 0);
            }
            else
            {
                character.characterInfo.characterScripts.characterAnimations.GetCharacterSprite().transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            float angle = Mathf.Atan2(character.characterInputs.characterActionsInfo.movement.x, character.characterInputs.characterActionsInfo.movement.y) * Mathf.Rad2Deg;
            directionPlayer.transform.rotation = Quaternion.Euler(0, angle, 0f);
        }
    }
    void MoveWhitCameraPc()
    {
        if (Camera.main.WorldToViewportPoint(character.characterInputs.mousePos.transform.position).x > 0.5f)
        {
            movementDirectionAnimation.x = 1;
            character.characterInfo.characterScripts.characterAnimations.GetCharacterSprite().transform.localRotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            movementDirectionAnimation.x = -1;
            character.characterInfo.characterScripts.characterAnimations.GetCharacterSprite().transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        if (Camera.main.WorldToViewportPoint(character.characterInputs.mousePos.transform.position).y > 0.5f)
        {
            movementDirectionAnimation.y = 1;
        }
        else
        {
            movementDirectionAnimation.y = -1;
        }
        float angle = Mathf.Atan2(character.characterInputs.characterActionsInfo.moveCamera.x, character.characterInputs.characterActionsInfo.moveCamera.y) * Mathf.Rad2Deg;
        directionPlayer.transform.rotation = Quaternion.Lerp(directionPlayer.transform.rotation, Quaternion.Euler(0, angle, 0f), 0.25f);
    }
    void MoveWhitJoystick()
    {
        if (character.characterInputs.characterActionsInfo.moveCamera != Vector2.zero)
        {
            if (character.characterInputs.characterActionsInfo.moveCamera.x > 0)
            {
                movementDirectionAnimation.x = 1;
                character.characterInfo.characterScripts.characterAnimations.GetCharacterSprite().transform.localRotation = Quaternion.Euler(0, -180, 0);
            }
            else
            {
                movementDirectionAnimation.x = -1;
                character.characterInfo.characterScripts.characterAnimations.GetCharacterSprite().transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            if (character.characterInputs.characterActionsInfo.moveCamera.y > 0)
            {
                movementDirectionAnimation.y = 1;
            }
            else
            {
                movementDirectionAnimation.y = -1;
            }
            float angle = Mathf.Atan2(character.characterInputs.characterActionsInfo.moveCamera.x, character.characterInputs.characterActionsInfo.moveCamera.y) * Mathf.Rad2Deg;
            directionPlayer.transform.rotation = Quaternion.Lerp(directionPlayer.transform.rotation, Quaternion.Euler(0, angle, 0f), 0.25f);
        }
    }
    void OnDrawGizmos()
    {
        if (directionPlayer != null)
        {
            Gizmos.color = Color.red;
            Vector3 endPoint = directionPlayer.transform.position + directionPlayer.transform.forward * rayDistanceTarget;
            Gizmos.DrawLine(directionPlayer.transform.position, endPoint);
            if (Physics.BoxCast(directionPlayer.transform.position, Vector3.one, directionPlayer.transform.forward, out RaycastHit objectHit, Quaternion.identity, rayDistanceTarget, targetMask))
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(objectHit.point, Vector3.one * 0.2f);
            }
            Gizmos.color = Color.yellow;
            Gizmos.matrix = Matrix4x4.TRS(directionPlayer.transform.position, Quaternion.LookRotation(directionPlayer.transform.forward), Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero + Vector3.forward * (rayDistanceTarget / 2), new Vector3(Vector3.one.x * 2, Vector3.one.y * 2, rayDistanceTarget));
        }
    }
    public Vector2 GetDirectionAnimation()
    {
        return movementDirectionAnimation;
    }
    public Character GetLookTarget()
    {
        return characterTarget;
    }
    public Vector2 GetDirectionMovementCharacter(){
        return movementCharacter;
    }
    public void SetTarget(GameObject target)
    {
        characterTarget = target.GetComponent<Character>();
    }
    public interface ICharacterDirection
    {
        public Vector2 GetDirectionMovementCharacter();
        public Vector2 GetDirectionAnimation();
        public Character GetLookTarget();
        public void SetTarget(GameObject target);
    }
}
