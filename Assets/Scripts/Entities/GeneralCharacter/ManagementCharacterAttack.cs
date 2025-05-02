using System.Collections;
using UnityEngine;

public class ManagementCharacterAttack : MonoBehaviour
{
    [SerializeField] Character character;
    [SerializeField] GameObject posisionAttack;
    ManagementCharacterModelDirection.ICharacterDirection characterDirection;
    float distAttack = 1;
    public float distLostTarget = 1;
    float cooldownAttack = 0;
    Character.Statistics costsAttack = new Character.Statistics(Character.TypeStatistics.Sp, 10, 0, 0, 0, 0);
    void Start()
    {
        characterDirection = GetComponent<ManagementCharacterModelDirection.ICharacterDirection>();
    }
    void Update()
    {
        if (cooldownAttack > 0)
        {
            cooldownAttack -= Time.deltaTime;
        }
    }
    public void ValidateAttack()
    {
        if (!character.characterInfo.isPlayer)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, distAttack, LayerMask.GetMask("Player"));
            if (hitColliders.Length > 0 && hitColliders[0].GetComponent<Character>().characterInfo.isActive)
            {
                if (character.characterInfo.characterScripts.characterAnimations != null && 
                    character.characterInfo.characterScripts.characterAnimations.ValidateAnimationEnd(ManagementCharacterAnimations.TypeAnimation.TakeDamage) &&
                    ValidateAllAnimationsAttackEnd() && cooldownAttack <= 0)
                {
                    characterDirection.SetTarget(hitColliders[0].gameObject);
                    StartAttack();
                }
            }
        }
        else
        {
            if (character.characterInfo.characterScripts.characterAnimations != null &&
                character.characterInfo.characterScripts.characterAnimations.ValidateAnimationEnd(ManagementCharacterAnimations.TypeAnimation.TakeDamage) &&
                ValidateAllAnimationsAttackEnd() && cooldownAttack <= 0 &&
                character.characterInfo.GetStatisticByType(costsAttack.typeStatistics).currentValue - costsAttack.baseValue >= 0)
            {
                character.characterInfo.GetStatisticByType(costsAttack.typeStatistics).currentValue -= costsAttack.baseValue;
                StartAttack();
            }
        }
    }
    void StartAttack()
    {
        character.characterInfo.characterScripts.characterAnimations.MakeAnimation(ValidateTypeAttack());
        SetCoolDown();
        Attack();
    }
    ManagementCharacterAnimations.TypeAnimation ValidateTypeAttack()
    {
        foreach(ManagementCharacterObjects.ObjectsInfo weapon in character.characterInfo.characterScripts.managementCharacterObjects.objects)
        {
            if (weapon.objectData && weapon.isUsingItem)
            {
                if (weapon.objectData.objectInstance.TryGetComponent<ManagementWeaponsObject>(out ManagementWeaponsObject weaponObject))
                {
                    return weaponObject.typeAnimation;
                }
            }
        }
        return ManagementCharacterAnimations.TypeAnimation.HandAttack;
    }
    bool ValidateAllAnimationsAttackEnd()
    {
        if (character.characterInfo.characterScripts.characterAnimations.GetCurrentAnimation().typeAnimation != ManagementCharacterAnimations.TypeAnimation.HandAttack &&
            character.characterInfo.characterScripts.characterAnimations.GetCurrentAnimation().typeAnimation != ManagementCharacterAnimations.TypeAnimation.SwordAttack &&
            character.characterInfo.characterScripts.characterAnimations.GetCurrentAnimation().typeAnimation != ManagementCharacterAnimations.TypeAnimation.SpearAttack &&
            character.characterInfo.characterScripts.characterAnimations.GetCurrentAnimation().typeAnimation != ManagementCharacterAnimations.TypeAnimation.BowAttack &&
            character.characterInfo.characterScripts.characterAnimations.GetCurrentAnimation().typeAnimation != ManagementCharacterAnimations.TypeAnimation.AxeAttack &&
            character.characterInfo.characterScripts.characterAnimations.GetCurrentAnimation().typeAnimation != ManagementCharacterAnimations.TypeAnimation.CrosierAttack
        ) return true;

        return false;
    }
    void SetCoolDown()
    {
        cooldownAttack = 1 / character.characterInfo.GetStatisticByType(Character.TypeStatistics.AtkSpd).currentValue;
    }
    void Attack()
    {
        StartCoroutine(WaitToAttack());
    }
    bool IsValidToMakeAttack()
    {
        if (character.characterInfo.characterScripts.characterAnimations.GetCurrentAnimation().typeAnimation != ManagementCharacterAnimations.TypeAnimation.HandAttack ||
            character.characterInfo.characterScripts.characterAnimations.GetCurrentAnimation().typeAnimation != ManagementCharacterAnimations.TypeAnimation.SwordAttack ||
            character.characterInfo.characterScripts.characterAnimations.GetCurrentAnimation().typeAnimation != ManagementCharacterAnimations.TypeAnimation.SpearAttack ||
            character.characterInfo.characterScripts.characterAnimations.GetCurrentAnimation().typeAnimation != ManagementCharacterAnimations.TypeAnimation.BowAttack ||
            character.characterInfo.characterScripts.characterAnimations.GetCurrentAnimation().typeAnimation != ManagementCharacterAnimations.TypeAnimation.AxeAttack ||
            character.characterInfo.characterScripts.characterAnimations.GetCurrentAnimation().typeAnimation != ManagementCharacterAnimations.TypeAnimation.CrosierAttack
        ) return false;

        return true;
    }
    IEnumerator WaitToAttack()
    {
        while (IsValidToMakeAttack())
        {
            if (character.characterInfo.characterScripts.characterAnimations.GetCurrentAnimation().typeAnimation == ManagementCharacterAnimations.TypeAnimation.TakeDamage)
            {
                StopAllCoroutines();
            }
            yield return null;
        }
        while (character.characterInfo.characterScripts.characterAnimations.GetCurrentAnimation().frameToInstance >= character.characterInfo.characterScripts.characterAnimations.GetAnimationsInfo().currentSpriteIndex)
        {
            yield return null;
        }
        if (character.characterInfo.characterScripts.characterAnimations.GetCurrentAnimation().typeAnimation != ManagementCharacterAnimations.TypeAnimation.TakeDamage)
        {
            GameObject instance = Instantiate(character.characterInfo.characterScripts.characterAnimations.GetCurrentAnimation().instanceObj, transform.position, Quaternion.identity);
            instance.layer = character.characterInfo.isPlayer ? LayerMask.NameToLayer("PlayerAttack") : LayerMask.NameToLayer("EnemyAttack");
            instance.GetComponent<ManagementInstanceAttack.IInstanceAttack>().SetDamage(character.characterInfo.GetStatisticByType(Character.TypeStatistics.Atk).currentValue);
            instance.GetComponent<ManagementInstanceAttack.IInstanceAttack>().SetObjectMakeDamage(character);
            instance.GetComponent<ManagementCharacterInstance>().SetInfoForAnimation(character.characterInfo.characterScripts.managementCharacterModelDirection.GetDirectionAnimation(), character.characterInfo.characterScripts.characterAnimations.GetAnimationsInfo());
            instance.transform.position = character.characterInfo.characterScripts.characterAttack.GetDirectionAttack().transform.position;
            instance.transform.rotation = character.characterInfo.characterScripts.characterAttack.GetDirectionAttack().transform.rotation;
            instance.transform.localScale = Vector3.one;
            character.characterInfo.PlayASound(CharacterSoundsSO.TypeSound.Slash, false);
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distAttack);
    }
    public float GetDistLostTarget()
    {
        return distLostTarget;
    }
    public GameObject GetDirectionAttack()
    {
        return posisionAttack;
    }
}
