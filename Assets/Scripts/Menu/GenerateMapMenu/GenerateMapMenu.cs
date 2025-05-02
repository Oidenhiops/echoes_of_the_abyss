using UnityEngine;

public class GenerateMapMenu : MonoBehaviour
{
    public ManagementMapBlock[] blocks;
    void Start()
    {
        foreach(var block in blocks)
        {
            block.DrawBlock();
        }
    }
}
