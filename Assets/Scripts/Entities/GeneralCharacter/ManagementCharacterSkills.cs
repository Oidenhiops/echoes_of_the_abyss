using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ManagementCharacterSkills : MonoBehaviour
{
    [SerializeField] Character character;
    [SerializeField] ManagementCharacterHud managementCharacterHud;
    [SerializeField] ManagementCharacterAnimations managementCharacterAnimations;
    [SerializeField] SkillInfo[] currentSkills = new SkillInfo[5];
    int currentSkillIndex = 0;
    bool usingSkill;
    public void InitializeSkillsEvents()
    {
        character.characterInputs.characterActions.CharacterInputs.ChangeSkill.performed += OnChangeSkillUp;
        character.characterInputs.characterActions.CharacterInputs.UseSkill.performed += OnUseSkill;
    }
    public void HandleSkills()
    {
        if (character.characterInfo.isActive)
        {
            if (currentSkills[0].skillData != null)
            {
                foreach (SkillInfo skill in currentSkills)
                {
                    if (skill.skillData != null && skill.cdInfo.currentCD > 0)
                    {
                        skill.cdInfo.currentCD -= Time.deltaTime;
                    }
                }
                managementCharacterHud.RefreshSkillsTimer(currentSkills);
            }
            if (usingSkill)
            {
                UseSkill();
            }
        }
    }
    void OnChangeSkillUp(InputAction.CallbackContext context){
        if (character.characterInfo.isActive){
            ChangeSkill(context.ReadValue<float>() > 0);
        }
    }
    void OnUseSkill(InputAction.CallbackContext context){
        if (context.action.triggered){
            ValidateUseSkill();
        }
    }
    void ChangeSkill(bool isUp)
    {
        currentSkillIndex += isUp ? 1 : -1;
        if (currentSkillIndex < 0)
        {
            currentSkillIndex = currentSkills.Length - 1;
        }
        else if (currentSkillIndex > currentSkills.Length - 1)
        {
            currentSkillIndex = 0;
        }
        managementCharacterHud.ChangeCurrentSkill(false, currentSkillIndex);
    }
    public void ChangeSkill(int pos)
    {
        currentSkillIndex = pos;
        managementCharacterHud.ChangeCurrentSkill(false, currentSkillIndex);
    }
    void ValidateUseSkill()
    {
        if (currentSkills[currentSkillIndex].skillData != null && currentSkills[currentSkillIndex].cdInfo.currentCD <= 0)
        {
            if (currentSkills[currentSkillIndex].skillData.isPorcent)
            {
                int value = currentSkills[currentSkillIndex].skillData.isFromBaseValue ?
                    (int)Mathf.Ceil(character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).baseValue) :
                    (int)Mathf.Ceil(character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue);
                int amount = (int)(value * currentSkills[currentSkillIndex].skillData.cost.baseValue / 100);

                if (currentSkills[currentSkillIndex].skillData.cost.typeStatistics == Character.TypeStatistics.Hp &&
                    character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue - amount > 1 ||
                    character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue - amount >= 0)
                {
                    character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue -= amount;
                    currentSkills[currentSkillIndex].cdInfo.currentCD = currentSkills[currentSkillIndex].cdInfo.cd;
                    InitializeUsingSkill();
                }
            }
            else if (currentSkills[currentSkillIndex].skillData.cost.typeStatistics == Character.TypeStatistics.Hp &&
                     character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue - currentSkills[currentSkillIndex].skillData.cost.baseValue >= 1 ||
                     character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue - currentSkills[currentSkillIndex].skillData.cost.baseValue >= 0)
            {
                int amount = (int)Mathf.Ceil(currentSkills[currentSkillIndex].skillData.cost.baseValue);
                character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue -= amount;
                currentSkills[currentSkillIndex].cdInfo.currentCD = currentSkills[currentSkillIndex].cdInfo.cd;
                InitializeUsingSkill();
            }
        }
    }
    void InitializeUsingSkill()
    {
        if (currentSkills[currentSkillIndex].skillData.needAnimation)
        {
            managementCharacterAnimations.MakeAnimation(CharacterAnimationsSO.TypeAnimation.Skill, currentSkills[currentSkillIndex].skillData.skillAnimation.animationName);
        }
        usingSkill = true;
    }
    void UseSkill()
    {
        if (!currentSkills[currentSkillIndex].skillData.needAnimation)
        {
            usingSkill = false;
        }
        else
        {
            if (managementCharacterAnimations.currentAnimation.typeAnimation == CharacterAnimationsSO.TypeAnimation.Skill)
            {
                if (managementCharacterAnimations.currentAnimation.frameToInstance == managementCharacterAnimations.characterAnimationsInfo.currentSpriteIndex && 
                    managementCharacterAnimations.currentAnimation.needInstance)
                {
                    usingSkill = false;
                    GameObject skill = Instantiate(currentSkills[currentSkillIndex].skillData.skillObject, transform.position, Quaternion.identity, transform);
                    skill.GetComponent<ISkill>().SendInformation(character.characterInfo.characterStatistics, character);
                }
            }
            else
            {
                usingSkill = false;
            }
        }
    }
    public void InitializeSkills()
    {
        if (GameData.Instance.saveData.gameInfo.characterInfo.characterSelected != null)
        {
            currentSkills = GameData.Instance.saveData.gameInfo.characterInfo.currentSkills;
        }
        for (int i = 0; i < currentSkills.Length; i++)
        {
            if (currentSkills[i].skillData != null)
            {
                currentSkills[i].cdInfo = new SkillDataScriptableObject.CdInfo
                {
                    cd = currentSkills[i].skillData.cdInfo.cd
                };
            }
        }
        managementCharacterHud.RefreshSkillsSprites(currentSkills);
        managementCharacterHud.ChangeCurrentSkill(false, 0);
    }
    [Serializable] public class SkillInfo
    {
        public SkillDataScriptableObject skillData;
        public SkillDataScriptableObject.CdInfo cdInfo = new SkillDataScriptableObject.CdInfo();
    }
    public interface ISkill
    {
        public void SendInformation(Dictionary<Character.TypeStatistics, Character.Statistics> statistics, Character character);
    }
}
