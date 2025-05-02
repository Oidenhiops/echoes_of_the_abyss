using UnityEngine;

public class ManagementMapDecoration : MonoBehaviour
{
    public Sprite[] spriteKeys;
    public Mesh mesh;
    public MeshRenderer meshRenderer;
    [NaughtyAttributes.Button]  public void DrawBlock(){
        SetTextureFromAtlas();
    }
    void SetTextureFromAtlas()
    {
        Mesh newMesh = GetMeshByTexture();
        Vector2[] uvs = meshRenderer.GetComponent<MeshFilter>().mesh.uv;        
        if (newMesh != null)
        {
            meshRenderer.GetComponent<MeshFilter>().mesh = newMesh;
            uvs = newMesh.uv;
            if (GetComponent<MeshCollider>() != null)
            {
                GetComponent<MeshCollider>().sharedMesh = newMesh;
            }
        }
        Texture2D texture = spriteKeys[Random.Range(0, spriteKeys.Length)].texture;
        meshRenderer.material.mainTexture = texture;
        Rect spriteRect = spriteKeys[Random.Range(0, spriteKeys.Length)].textureRect;
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i].x = Mathf.Lerp(spriteRect.x / texture.width, (spriteRect.x + spriteRect.width) / texture.width, uvs[i].x);
            uvs[i].y = Mathf.Lerp(spriteRect.y / texture.height, (spriteRect.y + spriteRect.height) / texture.height, uvs[i].y);
        }
        meshRenderer.GetComponent<MeshFilter>().mesh.uv = uvs;
    }
    public Mesh GetMeshByTexture()
    {
        Mesh copia = new Mesh();
        copia.vertices = mesh.vertices;
        copia.triangles = mesh.triangles;
        copia.uv = mesh.uv;
        copia.normals = mesh.normals;
        copia.colors = mesh.colors;
        copia.tangents = mesh.tangents;
        copia.bounds = mesh.bounds;
        copia.boneWeights = mesh.boneWeights;
        copia.bindposes = mesh.bindposes;
        return copia;
    }
}
