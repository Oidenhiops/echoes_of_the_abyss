using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ManagementSlashEffect : MonoBehaviour
{
    public VisualEffect slashEffect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ChangeParticleColors();
    }
    public void ChangeParticleColors()
    {
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Map")))
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();

            if (renderer != null)
            {
                if (renderer.material.mainTexture is Texture2D texture)
                {
                    Vector2 uv = hit.textureCoord;

                    int x = Mathf.FloorToInt(uv.x * texture.width);
                    int y = Mathf.FloorToInt(uv.y * texture.height);

                    Color pixelColor = texture.GetPixel(x, y);

                    slashEffect.SetVector4("GroundColor", pixelColor);
                }
                else
                {
                    Color materialColor = renderer.material.color;
                    slashEffect.SetVector4("GroundColor", materialColor);
                }
            }
            slashEffect.SetBool("ShowEffects", true);
        }
        else
        {
            slashEffect.SetBool("ShowEffects", false);
        }
    }
}