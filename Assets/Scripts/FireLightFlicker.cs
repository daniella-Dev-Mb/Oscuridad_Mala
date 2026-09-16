using UnityEngine;

public class FireLightFlicker : MonoBehaviour
{
    [SerializeField] private Light fireLight;

    [Header("Intensidad")]
    [SerializeField] private float minIntensity = 1.5f;
    [SerializeField] private float maxIntensity = 4f;

    [Header("Velocidad")]
    [SerializeField] private float flickerSpeed = 5f;

    private float randomOffset;

    void Start()
    {
        if (fireLight == null)
            fireLight = GetComponent<Light>();

        randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float noise = Mathf.PerlinNoise(
            Time.time * flickerSpeed,
            randomOffset
        );

        fireLight.intensity = Mathf.Lerp(
            minIntensity,
            maxIntensity,
            noise
        );
    }
}