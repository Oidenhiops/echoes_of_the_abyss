using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFollower : MonoBehaviour, Character.ICharacterMove
{
    [SerializeField] Character character;
    [SerializeField] ManagementCharacterModelDirection managementCharacterModelDirection;
    [SerializeField] Rigidbody rb;
    public AStarPathFinding aStarPathFinding;
    List<Vector3> currentPath = new List<Vector3>();
    public Character target;
    float distToDetectTarget = 20;
    int currentTargetIndex = 0;
    bool freeMovement = false;
    int minDistToFreeMove;
    float distToNextPathPoint = 0.5f;
    Vector2 movementDirection = Vector2.zero;
    bool isGrounded = false;
    Vector3 offsetCheckIsGrounded = new Vector3{y = 0.25f};
    Vector3 sizeCheckIsGrounded = new Vector3(0.25f, 0.1f, 0.25f);
    float rayDistance = 0.25f;
    public void Start()
    {
        aStarPathFinding = FindAnyObjectByType<AStarPathFinding>();
    }
    public void Move()
    {
        if (character == null || aStarPathFinding == null || rb.isKinematic) return;
        if (character.characterInfo.isActive)
        {
            isGrounded = CheckIsGrounded();
            if (target != null)
            {
                if (!target.characterInfo.isActive)
                {
                    target = null;
                }
                else
                {
                    if (Vector3.Distance(transform.position, target.transform.position) > minDistToFreeMove)
                    {
                        freeMovement = false;
                    }
                    else
                    {
                        //freeMovement = true;
                    }
                    if (!freeMovement)
                    {
                        if (target != null)
                        {
                            currentPath = aStarPathFinding.FindPath(FindClosestPosition(transform.position), FindClosestPosition(target.transform.position));
                            if (currentPath.Count > 0)
                            {
                                if (currentTargetIndex > currentPath.Count - 1)
                                {
                                    currentTargetIndex = currentPath.Count - 1;
                                }
                                MoveTowardsPath(currentPath[currentTargetIndex]);
                            }
                        }
                    }
                    else
                    {
                        MoveTowardsPath(target.transform.position);
                    }
                }
            }
            else
            {
                GetCharacterTarget(out Character selecterTarget);
                if (target != selecterTarget)
                {
                    target = selecterTarget;
                }
                movementDirection = Vector2.zero;
                managementCharacterModelDirection.movementCharacter = movementDirection;
            }
        }
    }
    Vector3 FindClosestPosition(Vector3 posToFind)
    {
        Vector3 closestPos = Vector3.zero;
        float closestDistance = Mathf.Infinity;
        foreach (Vector3 pos in aStarPathFinding.occupiedPositions)
        {
            float distance = Vector3.Distance(posToFind, pos);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPos = pos;
            }
        }
        return closestPos;
    }
    void MoveTowardsPath(Vector3 targetPosition)
    {
        if (character.characterInfo.characterScripts.characterAnimations.GetCurrentAnimation().typeAnimation != ManagementCharacterAnimations.TypeAnimation.TakeDamage && 
            !character.characterInfo.characterScripts.managementStatusEffect.statusEffects.ContainsKey(StatusEffectSO.TypeStatusEffect.Push))
        {
            Vector2 characterInputs = (new Vector2(targetPosition.x, targetPosition.z) - new Vector2(transform.position.x, transform.position.z)).normalized;
            if (characterInputs != Vector2.zero)
            {
                movementDirection = characterInputs;
            }
            else
            {
                movementDirection = Vector2.zero;
            }
            managementCharacterModelDirection.movementCharacter = movementDirection;
            Vector3 moveDirection = new Vector3(characterInputs.x * character.characterInfo.GetStatisticByType(Character.TypeStatistics.Spd).currentValue * 0.8f, rb.linearVelocity.y, characterInputs.y * character.characterInfo.GetStatisticByType(Character.TypeStatistics.Spd).currentValue * 0.8f);
            float dist = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetPosition.x, targetPosition.z));
            if (dist < distToNextPathPoint)
            {
                currentTargetIndex++;
                if (currentTargetIndex > currentPath.Count - 1)
                {
                    currentTargetIndex = 0;
                }
            }
            rb.linearVelocity = moveDirection;
        }
    }
    void GetCharacterTarget(out Character selectedTarget)
    {
        Collider[] hitCollidersArray = Physics.OverlapSphere(transform.position, distToDetectTarget, LayerMask.GetMask("Player"));
        List<Collider> hitColliders = hitCollidersArray.ToList();
        Collider posibleCharacter = null;
        if (hitColliders.Count > 0)
        {
            float minorDist = Mathf.Infinity;
            for (int i = hitColliders.Count - 1; i >= 0; i--)
            {
                if (hitColliders[i].GetComponent<Character>().characterInfo.isActive)
                {
                    float dist = Vector3.Distance(transform.position, hitColliders[i].transform.position);
                    if (dist < minorDist)
                    {
                        minorDist = dist;
                        posibleCharacter = hitColliders[i];
                    }
                }
                else
                {
                    hitColliders.Remove(hitColliders[i]);
                }
            }
        }
        selectedTarget = hitColliders.Count == 0 ? null : posibleCharacter.GetComponent<Character>();
    }
    bool CheckIsGrounded()
    {
        return Physics.BoxCast(transform.position + offsetCheckIsGrounded, sizeCheckIsGrounded, Vector3.down, out _, Quaternion.identity, rayDistance, LayerMask.GetMask("Map"));
    }
    public Rigidbody GetRigidbody()
    {
        return rb;
    }
    void OnDrawGizmos()
    {
        Vector3 center = transform.position + offsetCheckIsGrounded;
        RaycastHit hit;
        if (Physics.BoxCast(transform.position + offsetCheckIsGrounded, sizeCheckIsGrounded, Vector3.down, out hit, Quaternion.identity, rayDistance, LayerMask.GetMask("Map")))
        {
            Vector3 hitPos = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(center, hitPos);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(hitPos, sizeCheckIsGrounded);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(center, center + Vector3.down * rayDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(center + Vector3.down * rayDistance, sizeCheckIsGrounded);
        }


        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distToDetectTarget);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, minDistToFreeMove);
        if (currentPath == null || currentPath.Count == 0)
            return;
        Gizmos.color = Color.yellow;
        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Gizmos.DrawLine(currentPath[i] + Vector3.up, currentPath[i + 1] + Vector3.up);
        }
        foreach (var point in currentPath)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(point + Vector3.up, 0.1f);
        }
    }

    public void SetPositionTarget(Transform position){}
    public void SetCanMoveState(bool state){}
    public void SetTarget(Transform targetPos){}
}
