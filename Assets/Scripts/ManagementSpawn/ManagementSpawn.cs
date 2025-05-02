using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagementSpawn : MonoBehaviour
{
    public ManagementChunks managementChunks;
    [SerializeField] AStarPathFinding aStarPathFinding;
    public List<ManagementSpawnPosition> spawnPositions;
    public int amountSpawns = 5;
    public List<GameObject> currentEnemies = new List<GameObject>();
    public InitialDataSO[] characters;
    [SerializeField] GameObject characterRef;
    [SerializeField] GameObject spawnPositionRef;
    [SerializeField] SpawnTimeInfo[] spawnInfo;
    [SerializeField] int currentSpawn;
    void Start()
    {
        characters = Resources.LoadAll<InitialDataSO>("SciptablesObjects/Character/InitialData");
    }
    public void SetSpawnPositions()
    {
        for (int i = 0; i < amountSpawns; i++)
        {
            GameObject spawn = Instantiate(spawnPositionRef, GetPositions(), Quaternion.identity);
            spawnPositions.Add(spawn.GetComponent<ManagementSpawnPosition>());
        }
    }
    public void StartSpawn()
    {
        StartCoroutine(Spawn());
    }
    public IEnumerator Spawn()
    {
        while (true)
        {
            for (int i = 0; i < spawnInfo[currentSpawn].amountEnemies; i++)
            {
                GameObject character = Instantiate(characterRef, GetSpawnPosition(), Quaternion.identity);
                if (character.TryGetComponent<Character>(out Character characterScript))
                {
                    characterScript.characterInfo.initialData = GetCharacterInitialData();
                    characterScript.characterInfo.isActive = true;
                    character.GetComponent<PathFollower>().aStarPathFinding = aStarPathFinding;
                    character.GetComponent<PathFollower>().GetRigidbody().isKinematic = false;
                }
                currentEnemies.Add(character);
                character.SetActive(true);
            }
            yield return new WaitForSeconds(spawnInfo[currentSpawn].spawnDelay);
        }
    }
    Vector3 GetSpawnPosition()
    {
        return spawnPositions[Random.Range(0, spawnPositions.Count)].positionSpawn.transform.position;
    }
    InitialDataSO GetCharacterInitialData()
    {
        return characters[Random.Range(0, characters.Length)].Clone();
    }
    public Vector3 GetPositions()
    {
        Vector3 pos = managementChunks.allBlocksGenerated[Random.Range(0, managementChunks.allBlocksGenerated.Count)].transform.position;
        pos.y = 1;
        return pos;
    }
    [System.Serializable] class SpawnTimeInfo
    {
        public int maxTimeToSpawn = 0;
        public int spawnDelay = 0;
        public int amountEnemies = 0;
    }
}
