using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCharacterMenu : MonoBehaviour
{
    public StaticticsUi[] staticticsUi;
    public InitialDataSO characterSelected;
    public InitialDataSO[] allCharacters;
    public TMP_Text nameCharacter;
    public int currentIndex = 0;
    public Character character;
    public Image skillSprite;
    public ManagementLanguage skillText;
    public bool isSkillWindow = false;
    public bool isObjectsWindow = false;
    public GameObject containerObjects;
    public Button backButton;
    public Button playButton;

    public void OnEnable()
    {        
        allCharacters = Resources.LoadAll<InitialDataSO>("SciptablesObjects/Character/InitialData");
        characterSelected = GameData.Instance.saveData.gameInfo.characterInfo.characterSelected;
        GameData.Instance.saveData.gameInfo.characterInfo.currentSkills[0] = characterSelected.baseSkill;
        for (int i = 0; i < allCharacters.Length; i++)
        {
            if (characterSelected == allCharacters[i])
            {
                currentIndex = i;
                break;
            }
        }
        SetCharacterData();
        backButton.Select();
    }
    void OnDisable()
    {
        playButton.Select();
    }
    public void SetCharacterData()
    {
        for (int u = 0; u < staticticsUi.Length; u++)
        {
            for (int s = 0; s < characterSelected.characterInfo.characterStatistics.Count; s++)
            {
                if (staticticsUi[u].typeStatistics == characterSelected.characterInfo.characterStatistics.ElementAt(s).Value.typeStatistics)
                {
                    staticticsUi[u].statisticsText.text = characterSelected.characterInfo.characterStatistics.ElementAt(s).Value.baseValue.ToString();
                }
            }
        }
        nameCharacter.text = Regex.Replace(characterSelected.name, "(?<=\\p{Ll})(\\p{Lu})", " $1");
        skillSprite.sprite = characterSelected.baseSkill.skillData.skillSprite;
        skillText.id = characterSelected.baseSkill.skillData.textId;      
        SetObjectsData();
    }
    public void SetObjectsData()
    {
        for (int i = containerObjects.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(containerObjects.transform.GetChild(i).gameObject);
        }
        GameData.Instance.saveData.gameInfo.characterInfo.currentObjects = new ManagementCharacterObjects.ObjectsInfo[6];
        for (int i = 0; i < characterSelected.objects.Length; i++)
        {
            GameData.Instance.saveData.gameInfo.characterInfo.currentObjects[i] = new ManagementCharacterObjects.ObjectsInfo{
                objectPos = characterSelected.objects[i].objectPos,
                objectData = characterSelected.objects[i].objectData,
                objectId = characterSelected.objects[i].objectData.objectId,
                amount = characterSelected.objects[i].amount,
                isUsingItem = characterSelected.objects[i].isUsingItem,
            };
            GameObject characterObjectInstance = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Menu/ChangeCharacterObjects/ObjectBackground"), containerObjects.transform);
            ManagementChangeCharacterObject characterObject = characterObjectInstance.GetComponent<ManagementChangeCharacterObject>();
            characterObject.SetObjectData(characterSelected.objects[i].objectData.objectSprite, characterSelected.objects[i].amount);
        }
    }
    public void ChangeCharacter(bool direction)
    {
        if (direction)
        {
            currentIndex++;
        }
        else
        {
            currentIndex--;
        }
        if (currentIndex > allCharacters.Length - 1)
        {
            currentIndex = 0;
        }
        else if (currentIndex < 0)
        {
            currentIndex = allCharacters.Length - 1;
        }
        characterSelected = allCharacters[currentIndex];
        GameData.Instance.saveData.gameInfo.characterInfo.characterSelected = characterSelected;
        GameData.Instance.saveData.gameInfo.characterInfo.currentSkills[0] = characterSelected.baseSkill;
        GameData.Instance.saveData.gameInfo.characterInfo.characterSelectedName = characterSelected.name;
        GameData.Instance.SaveGameData();
        SetCharacterSprite();
        SetCharacterData();
    }
    public void ChangeCurrentWindow(int window)
    {
        switch (window)
        {
            case 0:
                isObjectsWindow = false;
                break;
            case 1:
                isObjectsWindow = true;
                break;
            case 2:
                isObjectsWindow = true;
                break;
        }
    }
    public void SetCharacterSprite()
    {
        character.characterInfo.characterScripts.characterAnimations.SetInitialData(characterSelected);
    }

    [System.Serializable]
    public class StaticticsUi
    {
        public Character.TypeStatistics typeStatistics;
        public TMP_Text statisticsText;
    }
}
