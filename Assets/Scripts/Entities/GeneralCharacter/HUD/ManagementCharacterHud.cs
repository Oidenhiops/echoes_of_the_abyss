using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;
using AYellowpaper.SerializedCollections;

public class ManagementCharacterHud : MonoBehaviour
{
    public Character character;
    public CharacterUi characterUi;
    public void InitializeHud()
    {
        if (character.characterInfo.isPlayer){
            GameManager.Instance.OnDeviceChanged += EnabledMobileHUD;
            character.characterInputs.characterActionsInfo.OnSecondaryActionChange += ToggleSecondaryAction;
            EnabledMobileHUD(GameManager.Instance.currentDevice);
        }
    }
    void OnDestroy()
    {
        
    }
    void EnabledMobileHUD(GameManager.TypeDevice typeDevice)
    {
        bool isActie = typeDevice == GameManager.TypeDevice.MOBILE;
        foreach (GameObject hud in characterUi.mobileHud)
        {
            hud.SetActive(isActie);
        }
    }
    void ToggleSecondaryAction(bool value)
    {
        characterUi.objectsUi.containerObjects.color = !value ? Color.white : characterUi.objectsUi.containerObjectsSecondaryActionColor;
    }
    public void HandleHud()
    {
        if (characterUi.hudUi.healthBarFront != null)
        {
            characterUi.hudUi.healthBarFront.fillAmount = Mathf.Lerp(characterUi.hudUi.healthBarFront.fillAmount, character.characterInfo.GetStatisticByType(Character.TypeStatistics.Hp).currentValue / character.characterInfo.GetStatisticByType(Character.TypeStatistics.Hp).maxValue, 1.5f * Time.deltaTime);
            if (characterUi.hudUi.healthBarBack.fillAmount >= characterUi.hudUi.healthBarFront.fillAmount)
            {
                characterUi.hudUi.healthBarBack.fillAmount = Mathf.Lerp(characterUi.hudUi.healthBarBack.fillAmount, character.characterInfo.GetStatisticByType(Character.TypeStatistics.Hp).currentValue / character.characterInfo.GetStatisticByType(Character.TypeStatistics.Hp).maxValue, 1 * Time.deltaTime);
            }
            else
            {
                characterUi.hudUi.healthBarBack.fillAmount = characterUi.hudUi.healthBarFront.fillAmount;
            }
            if (characterUi.hudUi.healthText != null)
            {
                characterUi.hudUi.healthText.text = $"{character.characterInfo.GetStatisticByType(Character.TypeStatistics.Hp).currentValue}/{character.characterInfo.GetStatisticByType(Character.TypeStatistics.Hp).maxValue}";
            }
        }
        if (characterUi.hudUi.manaBarFront != null)
        {
            characterUi.hudUi.manaBarFront.fillAmount = Mathf.Lerp(characterUi.hudUi.manaBarFront.fillAmount, character.characterInfo.GetStatisticByType(Character.TypeStatistics.Mp).currentValue / character.characterInfo.GetStatisticByType(Character.TypeStatistics.Mp).maxValue, 1.5f * Time.deltaTime);
            if (characterUi.hudUi.manaBarBack.fillAmount >= characterUi.hudUi.manaBarFront.fillAmount)
            {
                characterUi.hudUi.manaBarBack.fillAmount = Mathf.Lerp(characterUi.hudUi.manaBarBack.fillAmount, character.characterInfo.GetStatisticByType(Character.TypeStatistics.Mp).currentValue / character.characterInfo.GetStatisticByType(Character.TypeStatistics.Mp).maxValue, 1 * Time.deltaTime);
            }
            else
            {
                characterUi.hudUi.manaBarBack.fillAmount = characterUi.hudUi.manaBarFront.fillAmount;
            }
            if (characterUi.hudUi.manaText != null)
            {
                characterUi.hudUi.manaText.text = $"{character.characterInfo.GetStatisticByType(Character.TypeStatistics.Mp).currentValue}/{character.characterInfo.GetStatisticByType(Character.TypeStatistics.Mp).maxValue}";
            }
        }
        if (characterUi.hudUi.staminaBarFront != null)
        {
            characterUi.hudUi.staminaBarFront.fillAmount = Mathf.Lerp(characterUi.hudUi.staminaBarFront.fillAmount, character.characterInfo.GetStatisticByType(Character.TypeStatistics.Sp).currentValue / character.characterInfo.GetStatisticByType(Character.TypeStatistics.Sp).maxValue, 1.5f * Time.deltaTime);
            if (characterUi.hudUi.staminaBarBack.fillAmount >= characterUi.hudUi.staminaBarFront.fillAmount)
            {
                characterUi.hudUi.staminaBarBack.fillAmount = Mathf.Lerp(characterUi.hudUi.staminaBarBack.fillAmount, character.characterInfo.GetStatisticByType(Character.TypeStatistics.Sp).currentValue / character.characterInfo.GetStatisticByType(Character.TypeStatistics.Sp).maxValue, 1 * Time.deltaTime);
            }
            else
            {
                characterUi.hudUi.staminaBarBack.fillAmount = characterUi.hudUi.staminaBarFront.fillAmount;
            }
            if (characterUi.hudUi.staminaText != null)
            {
                characterUi.hudUi.staminaText.text = $"{character.characterInfo.GetStatisticByType(Character.TypeStatistics.Sp).currentValue}/{character.characterInfo.GetStatisticByType(Character.TypeStatistics.Sp).maxValue}";
            }
        }
        if (characterUi.statusEffectsUi.statusEffectsData.Count > 0)
        {
            foreach (var status in characterUi.statusEffectsUi.statusEffectsData)
            {
                status.Value.statusEffectUi.statusEffectAccumulations.text = status.Value.statusEffectsData.currentAccumulations > 1 ? status.Value.statusEffectsData.currentAccumulations.ToString() : "";
                float realValueValue = status.Value.statusEffectsData.currentTime - status.Value.statusEffectsData.statusEffectSO.timePerAcumulation * (status.Value.statusEffectsData.currentAccumulations - 1);
                status.Value.statusEffectUi.statusEffectFill.fillAmount = realValueValue / status.Value.statusEffectsData.statusEffectSO.timePerAcumulation;
            }
        }
        if (character.characterInputs.characterActions.CharacterInputs.ShowStats.triggered)
        {
            ToggleShowStatistics();
        }
    }
    public void SendInformationMessage(string messageText, Color color)
    {
        GameObject message = Instantiate(Resources.Load<GameObject>("Prefabs/UI/InformationMessage/InformationMessage"), characterUi.informationMessageUi.containerInformationMessage.transform);
        message.GetComponent<InformationMessages>().textMessage.text = messageText;
        message.GetComponent<InformationMessages>().textMessage.color = color;
        Destroy(message, 3);
    }
    public void ChangeObject(int objectIndex)
    {
        if (characterUi.objectsUi.objects[0].objectBackground != null)
        {
            for (int i = 0; i < characterUi.objectsUi.objects.Length; i++)
            {
                if (i == objectIndex)
                {
                    characterUi.objectsUi.objects[i].objectBackground.color = Color.yellow;
                }
                else
                {
                    characterUi.objectsUi.objects[i].objectBackground.color = Color.white;
                }
            }
        }
    }
    public void RefreshObjects(ManagementCharacterObjects.ObjectsInfo[] objects)
    {
        for (int i = 0; i < characterUi.objectsUi.objects.Length; i++)
        {
            if (objects[i].objectData != null)
            {
                characterUi.objectsUi.objects[i].spriteObject.sprite = objects[i].objectData.objectSprite;
                characterUi.objectsUi.objects[i].spriteObject.gameObject.SetActive(true);
                if (objects[i].amount > 1)
                {
                    characterUi.objectsUi.objects[i].amount.gameObject.SetActive(true);
                }
                else
                {
                    characterUi.objectsUi.objects[i].amount.gameObject.SetActive(false);
                }
                characterUi.objectsUi.objects[i].amount.text = objects[i].amount.ToString();
            }
            else
            {
                characterUi.objectsUi.objects[i].spriteObject.gameObject.SetActive(false);
                characterUi.objectsUi.objects[i].amount.gameObject.SetActive(false);
            }
        }
    }
    public void ToggleActiveObject(int pos, bool state)
    {        
        characterUi.objectsUi.objects[pos].usingObjectSprite.gameObject.SetActive(state);
    }
    public void RefreshObjectsForTake(GameObject[] objectsForTake)
    {
        foreach (Transform child in characterUi.objectsUi.containerTakeObjects.transform)
        {
            Destroy(child.gameObject);
        }
        characterUi.objectsUi.objectsForTake.Clear();

        for (int i = 0; i < objectsForTake.Length; i++)
        {
            GameObject bannerObject = Instantiate(Resources.Load<GameObject>("Prefabs/UI/BannerTakeObjects/BannerTakeObjects"), characterUi.objectsUi.containerTakeObjects.transform);
            ObjectsForTake objectForTake = new ObjectsForTake
            {
                objectForTake = objectsForTake[i],
                bannerObjectForTake = bannerObject
            };
            if (bannerObject.TryGetComponent<BannerTakeObjects>(out BannerTakeObjects bannerTakeObjects))
            {
                bannerTakeObjects.objectForTake = objectsForTake[i];
                bannerTakeObjects.managementCharacterObjects = character.characterInfo.characterScripts.managementCharacterObjects;
                bannerTakeObjects.managementLanguage.currentLanguage = GameData.Instance.saveData.configurationsInfo.currentLanguage;
                bannerTakeObjects.textObject.gameObject.SetActive(true);                
                if (objectsForTake[i].TryGetComponent<ManagementObject>(out ManagementObject managementObject))
                {
                    bannerTakeObjects.spriteObject.sprite = managementObject.objectInfo.objectData.objectSprite;
                    bannerTakeObjects.managementLanguage.id = managementObject.managementInteract.IDText;
                }
                else if (objectsForTake[i].TryGetComponent<ManagementInteract>(out ManagementInteract managementInteract))
                {
                    bannerTakeObjects.managementLanguage.id = managementInteract.IDText;
                }
            }
            characterUi.objectsUi.objectsForTake.Add(objectForTake);
        }        
    }
    public void RefreshCurrentObjectForTake(GameObject[] objectsForTake,GameObject currentObjectForTake,int currentObjectForTakeIndex)
    {
        if (GameManager.Instance.currentDevice != GameManager.TypeDevice.MOBILE)
        {
            foreach (ObjectsForTake currentObject in characterUi.objectsUi.objectsForTake)
            {
                if (currentObject.objectForTake == currentObjectForTake)
                {
                    BannerTakeObjects bannerTakeObjects = currentObject.bannerObjectForTake.GetComponent<BannerTakeObjects>();
                    bannerTakeObjects.takeButton.SetActive(true);
                    bannerTakeObjects.backgroundObject.color = Color.yellow;
                }
                else
                {
                    BannerTakeObjects bannerTakeObjects = currentObject.bannerObjectForTake.GetComponent<BannerTakeObjects>();
                    bannerTakeObjects.takeButton.SetActive(false);
                    bannerTakeObjects.backgroundObject.color = Color.white;
                }
            }
        }
        float contentHeight = objectsForTake.Length * 50;
        float viewportHeight = characterUi.objectsUi.bannerTakeObjectsScrollRect.viewport.rect.height;
        float targetPosition = currentObjectForTakeIndex * 50;
        targetPosition = Mathf.Clamp(targetPosition, 0, contentHeight - viewportHeight);
        float normalizedPosition = 1 - (targetPosition / (contentHeight - viewportHeight));
        characterUi.objectsUi.bannerTakeObjectsScrollRect.verticalNormalizedPosition = normalizedPosition;
    }
    public void RefreshCurrentStatistics()
    {
        foreach (StatisticsData statisticsData in characterUi.statisticsUi.statistics)
        {
            statisticsData.amount.text = character.characterInfo.GetStatisticByType(statisticsData.typeStatistic).currentValue.ToString();
        }
    }
    public void RefreshSkillsSprites(ManagementCharacterSkills.SkillInfo[] skills)
    {
        for (int i = 0; i < characterUi.skillsUi.skills.Length; i++)
        {
            if (skills[i].skillData != null)
            {
                characterUi.skillsUi.skills[i].skillSprite.sprite = skills[i].skillData.skillSprite;
                characterUi.skillsUi.skills[i].skillSprite.gameObject.SetActive(true);
            }
            else
            {
                characterUi.skillsUi.skills[i].skillSprite.gameObject.SetActive(false);
            }
        }
    }
    public void RefreshSkillsTimer(ManagementCharacterSkills.SkillInfo[] skills)
    {
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i].skillData != null)
            {
                characterUi.skillsUi.skills[i].skillTimer.text = skills[i].cdInfo.currentCD > 0 ? Math.Round(skills[i].cdInfo.currentCD, 2).ToString() : "";
                characterUi.skillsUi.skills[i].skillFillamount.fillAmount = skills[i].cdInfo.currentCD / skills[i].cdInfo.cd;
            }
            else
            {
                characterUi.skillsUi.skills[i].skillTimer.text = "";
                characterUi.skillsUi.skills[i].skillFillamount.fillAmount = 0;
            }
        }
    }
    public void ChangeCurrentSkill(bool desactiveSkillPos,int pos)
    {
        for (int i = 0; i < characterUi.skillsUi.skills.Length; i++)
        {
            if (!desactiveSkillPos && i == pos) 
            {
                characterUi.skillsUi.skills[i].skillBackground.color = Color.yellow;
            }
            else
            {
                characterUi.skillsUi.skills[i].skillBackground.color = Color.white;
            }
        }
    }
    public void UpdateStatusEffect(ManagementStatusEffect.StatusEffectsData statusEffectsData)
    {
        if (!characterUi.statusEffectsUi.statusEffectsData.ContainsKey(statusEffectsData.statusEffectSO.typeStatusEffect))
        {
            StatusEffectUiHelper statusEffectsUi = Instantiate(Resources.Load<GameObject>("Prefabs/UI/StatusEffect/StatusEffectUi"), characterUi.statusEffectsUi.statusEffectContainer).GetComponent<StatusEffectUiHelper>();
            characterUi.statusEffectsUi.statusEffectsData.Add(   
                statusEffectsData.statusEffectSO.typeStatusEffect,
                new StatusEffectsData(
                    statusEffectsUi,
                    statusEffectsData
                )
            );
        }
    }
    public void DestroyStatusEffect(StatusEffectSO.TypeStatusEffect typeStatusEffect){
        if (characterUi.statusEffectsUi.statusEffectsData.TryGetValue(typeStatusEffect, out StatusEffectsData statusEffectsData)){
            Destroy(statusEffectsData.statusEffectUi.gameObject);
            characterUi.statusEffectsUi.statusEffectsData.Remove(typeStatusEffect);
        }
    }
    void ToggleShowStatistics()
    {
        bool isOpen = !characterUi.statisticsUi.mStatistics.GetBool("IsOpen");
        characterUi.statisticsUi.mStatistics.SetBool("IsOpen", isOpen);
    }
    [Serializable] public class CharacterUi
    {
        public HudUi hudUi;
        public ObjectsUi objectsUi;
        public SkillsUi skillsUi;
        public StatisticsUi statisticsUi;
        public StatusEffectsUi statusEffectsUi;
        public InformationMessageUi informationMessageUi;
        public GameObject[] mobileHud;
        public CharacterUi (HudUi hudUi, ObjectsUi objectsUi, SkillsUi skillsUi, StatisticsUi statisticsUi, StatusEffectsUi statusEffectsUi, InformationMessageUi informationMessageUi)
        {
            this.hudUi = hudUi;
            this.objectsUi = objectsUi;
            this.skillsUi = skillsUi;
            this.statisticsUi = statisticsUi;
            this.statusEffectsUi = statusEffectsUi;
            this.informationMessageUi = informationMessageUi;
        }
    }
    [Serializable] public class HudUi
    {
        public Image healthBarBack;
        public Image healthBarFront;
        public TMP_Text healthText;
        public Image manaBarBack;
        public Image manaBarFront;
        public TMP_Text manaText;
        public Image staminaBarBack;
        public Image staminaBarFront;
        public TMP_Text staminaText;
    }
    [Serializable] public class ObjectsUi
    {
        public ObjectsData[] objects = new ObjectsData[6];
        public GameObject bannerTakeObjects;
        public GameObject containerTakeObjects;
        public Image containerObjects;
        public Color containerObjectsSecondaryActionColor;
        public List<ObjectsForTake> objectsForTake = new List<ObjectsForTake>();
        public ScrollRect bannerTakeObjectsScrollRect;
    }
    [Serializable] public class ObjectsForTake
    {
        public GameObject objectForTake;
        public GameObject bannerObjectForTake;
    }
    [Serializable] public class ObjectsData
    {
        public Image objectBackground;
        public Image spriteObject;
        public Image usingObjectSprite;
        public TMP_Text amount;
    }
    [Serializable] public class SkillsUi
    {
        public SkillsData[] skills = new SkillsData[4];
    }
    [Serializable] public class SkillsData
    {
        public Image skillBackground;
        public Image skillSprite;
        public Image skillFillamount;
        public TMP_Text skillTimer;
        public bool canActiveSkillSecondarySprite;
    }
    [Serializable] public class StatisticsUi
    {
        public Animator mStatistics;
        public StatisticsData[] statistics = new StatisticsData[]{
            new StatisticsData(null, null, Character.TypeStatistics.Atk),
            new StatisticsData(null, null, Character.TypeStatistics.Int),
            new StatisticsData(null, null, Character.TypeStatistics.AtkSpd),
            new StatisticsData(null, null, Character.TypeStatistics.Def),
            new StatisticsData(null, null, Character.TypeStatistics.Res),
            new StatisticsData(null, null, Character.TypeStatistics.Spd),
            new StatisticsData(null, null, Character.TypeStatistics.Crit),
            new StatisticsData(null, null, Character.TypeStatistics.CritDmg),
        };
        public StatisticsUi(Animator mStatistics, StatisticsData[] statistics)
        {
            this.mStatistics = mStatistics;
            this.statistics = statistics;
        }
    }
    [Serializable] public class StatisticsData
    {
        public Image sprite;
        public TMP_Text amount;
        public Character.TypeStatistics typeStatistic;

        public StatisticsData(Image sprite, TMP_Text amount, Character.TypeStatistics typeStatistic)
        {
            this.sprite = sprite;
            this.amount = amount;
            this.typeStatistic = typeStatistic;
        }
    }
    [Serializable] public class InformationMessageUi
    {
        public GameObject containerInformationMessage;
    }
    [Serializable] public class StatusEffectsUi
    {
        public Transform statusEffectContainer;
        public SerializedDictionary<StatusEffectSO.TypeStatusEffect, StatusEffectsData> statusEffectsData = new SerializedDictionary<StatusEffectSO.TypeStatusEffect, StatusEffectsData>();
    }
    [Serializable] public class StatusEffectsData
    {
        public StatusEffectUiHelper statusEffectUi;
        public ManagementStatusEffect.StatusEffectsData statusEffectsData;

        public StatusEffectsData(StatusEffectUiHelper statusEffectUi, ManagementStatusEffect.StatusEffectsData statusEffectsData)
        {
            this.statusEffectUi = statusEffectUi;
            this.statusEffectsData = statusEffectsData;
        }
    }
}