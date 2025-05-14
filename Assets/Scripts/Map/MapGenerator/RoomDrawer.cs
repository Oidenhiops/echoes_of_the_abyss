using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Unity.AI.Navigation;
using UnityEngine;

public class RoomDrawer : MonoBehaviour
{
    [SerializeField] NavMeshSurface navMeshSurface;
    public GameObject blocksContainer;
    public List<MapBlock> mapBlocks = new List<MapBlock>();
    public List<MapDecoration> decorationsBlocks = new List<MapDecoration>();
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
        DrawBlocks();
        yield return new WaitForSeconds(0.1f);
        BuildNavMesh();
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
        public MapBlock[] blocks;
    }
    public enum DirectionBridges
    {
        Forward = 0,
        Back = 1,
        Left = 2,
        Rigth = 3
    }
}
