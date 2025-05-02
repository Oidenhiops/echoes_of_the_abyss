using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagementRenderChunks : MonoBehaviour
{
    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Chunk"))
        {
            ManagementChunk chunk = other.GetComponent<ManagementChunk>();
            chunk.EnabledCombinedMesh();
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Chunk"))
        {
            ManagementChunk chunk = other.GetComponent<ManagementChunk>();
            chunk.DisableCombinedMesh();
        }
    }
}
