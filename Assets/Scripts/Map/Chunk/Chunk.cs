using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public int chunkSize;
    public RoomDrawer drawerMap;
    public bool autoDrawChunk = false;
    void Start()
    {
        if (autoDrawChunk) DrawRoom();
    }
    [NaughtyAttributes.Button]
    public void DrawRoom()
    {
        if (drawerMap == null) drawerMap = GetAndInstanceRandomRoom();
        drawerMap.DrawMap();
        StartCoroutine(CombineMeshes());
    }
    public RoomDrawer GetAndInstanceRandomRoom(){
        GameObject[] rooms = Resources.LoadAll<GameObject>("Prefabs/Map/AncientWall/Rooms");
        GameObject room = Instantiate(rooms[Random.Range(0, rooms.Length)], transform.position, Quaternion.identity, transform);
        return room.GetComponent<RoomDrawer>();
    }
    public IEnumerator CombineMeshes()
    {
        yield return new WaitForSeconds(0.2f);
        Dictionary<Texture2D, List<GameObject>> materialToCombineInstances = new Dictionary<Texture2D, List<GameObject>>();
        for (int i = 0; i < drawerMap.blocksRender.Count; i++)
        {
            Texture2D material = (Texture2D)drawerMap.blocksRender[i].material.GetTexture("_BaseMap");
            if (!materialToCombineInstances.ContainsKey(material))
            {
                materialToCombineInstances[material] = new List<GameObject>();
            }
            materialToCombineInstances[material].Add(drawerMap.blocksRender[i].gameObject);
        }
        foreach (var entry in materialToCombineInstances)
        {
            List<GameObject> bloks = entry.Value;
            CombineInstance[] combineInstances = new CombineInstance[bloks.Count];
            for (int i = 0; i < bloks.Count; i++)
            {
                MeshFilter meshFilter = bloks[i].GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    combineInstances[i].mesh = meshFilter.sharedMesh;
                    combineInstances[i].transform = bloks[i].transform.localToWorldMatrix;
                }
            }
            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combineInstances, true, true);
            GameObject combinedObject = new GameObject();
            combinedObject.layer = LayerMask.NameToLayer("Map");
            combinedObject.transform.SetParent(transform);
            MeshFilter combinedMeshFilter = combinedObject.AddComponent<MeshFilter>();
            combinedMeshFilter.mesh = combinedMesh;
            MeshRenderer combinedMeshRenderer = combinedObject.AddComponent<MeshRenderer>();
            combinedObject.AddComponent<MeshCollider>();
            combinedMeshRenderer.material = bloks[0].GetComponent<MeshRenderer>().material;
            combinedObject.name = $"CombinedMesh {bloks[0].GetComponent<MeshRenderer>().material.name}";
            foreach (GameObject obj in bloks)
            {
                Destroy(obj);
            }
        }
        DisableCombinedMesh();
    }
    [NaughtyAttributes.Button]
    public void DisableCombinedMesh()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer)){
                meshRenderer.enabled = false;
            }
            else{
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
    [NaughtyAttributes.Button]
    public void EnabledCombinedMesh()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer)){
                meshRenderer.enabled = true;
            }
            else{
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, chunkSize * Vector3.one);
    }
}
