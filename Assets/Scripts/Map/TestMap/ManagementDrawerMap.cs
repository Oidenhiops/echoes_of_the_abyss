using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Unity.AI.Navigation;
using UnityEngine;

public class ManagementDrawerMap : MonoBehaviour
{
    [SerializeField] NavMeshSurface navMeshSurface;
    [NonSerialized] public List<ManagementMapBlock> mapBlocks = new List<ManagementMapBlock>();
    public GameObject blocksContainer;
    List<ManagementMapDecoration> decorationsBlocks = new List<ManagementMapDecoration>();
    public List<MeshRenderer> blocksRender = new List<MeshRenderer>();
    public SerializedDictionary<DirectionBridges, BridgesInfo> bridges;
    public bool autoInit;
    void Start()
    {
        if (autoInit) DrawMap();
    }
    [NaughtyAttributes.Button]
    public void DrawMap()
    {
        StartCoroutine(DrawRoom());
    }
    public IEnumerator DrawRoom()
    {
        GetAllBlocks();
        yield return new WaitForSeconds(0.1f);
        DrawBlocks();
        yield return new WaitForSeconds(0.1f);
        BuildNavMesh();
    }
    void GetAllBlocks()
    {
        for (int i = 0; i < blocksContainer.transform.childCount; i++)
        {
            if (blocksContainer.transform.GetChild(i).gameObject.TryGetComponent<ManagementMapBlock>(out ManagementMapBlock managementMapBlock))
            {
                mapBlocks.Add(managementMapBlock);
            }
            else if (blocksContainer.transform.GetChild(i).gameObject.TryGetComponent<ManagementMapDecoration>(out ManagementMapDecoration managementMapSetTexture))
            {
                decorationsBlocks.Add(managementMapSetTexture);
            }
        }
    }
    void DrawBlocks()
    {
        for (int i = 0; i < mapBlocks.Count; i++)
        {
            mapBlocks[i].DrawBlock();
        }
        for (int i = 0; i < decorationsBlocks.Count; i++)
        {
            decorationsBlocks[i].DrawBlock();
        }
        mapBlocks.Clear();
        decorationsBlocks.Clear();
    }
    void BuildNavMesh()
    {
        //navMeshSurface.BuildNavMesh();
    }
    void UpdateNavMesh()
    {
        //navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
    }
    [Serializable] public class BridgesInfo
    {
        public GameObject bridge;
        public ManagementMapBlock[] blocks;
    }
    public enum DirectionBridges
    {
        Forward = 0,
        Back = 1,
        Left = 2,
        Rigth = 3
    }
}
