using System;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public GameObject blockWalls;
    int _amountEnemies;
    public Action<int> OnAllEnemiesDie;
    public int amountSpawns;
    public int amountEnemies
    {
        get => _amountEnemies;
        set
        {
            if (_amountEnemies != value)
            {
                _amountEnemies = value;
                if (_amountEnemies == 0)
                {
                    OnAllEnemiesDie?.Invoke(_amountEnemies);
                }
            }
        }
    }
    public void InitializeFigth()
    {
        amountSpawns = UnityEngine.Random.Range(2, 5);
        blockWalls.SetActive(true);
        SpawnEnemies();
    }
    public void ValidateAllEnemiesDie(int currentEnemies)
    {
        if (amountSpawns == 0)
        {
            FinishBattle();
        }
        else
        {
            SpawnEnemies();
        }
    }
    public void SpawnEnemies()
    {
        amountSpawns--;
        amountEnemies = UnityEngine.Random.Range(4, 10);
        print(amountEnemies);
    }
    void FinishBattle()
    {
        blockWalls.SetActive(false);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(GetComponent<BoxCollider>());
            InitializeFigth();
        }
    }
}
