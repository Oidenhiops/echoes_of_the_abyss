using UnityEngine;

public class ManagementLigthsObjects : MonoBehaviour
{
    public Light ligth;
    float minIntensity = 2f;
    float maxIntensity = 5f;
    public float speed = 2f; 

    private float time;

    void Start()
    {
        minIntensity = ligth.intensity - 5;
        if (minIntensity < 0) minIntensity = 0;
        maxIntensity = ligth.intensity;
        ligth.intensity = minIntensity;
    }

    void Update()
    {
        time += Time.deltaTime * speed;
        ligth.intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PingPong(time, 1));
    }
}
