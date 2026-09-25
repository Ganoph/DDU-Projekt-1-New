using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    private Light light;
    public float minIntensity = 0.5f;
    public float maxIntensity = 5.0f;
    public float flickerSpeed = 0.1f;

    private void Start()
    {
        light = GetComponent<Light>();
        InvokeRepeating(nameof(Flicker), 0f, flickerSpeed);
    }

    private void Flicker()
    {
        if (light != null)
            light.intensity = Random.Range(minIntensity, maxIntensity);
    }
}