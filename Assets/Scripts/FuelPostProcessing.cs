using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FuelPostProcessing : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Volume globalVolume;

    [Header("Aberracion cromatica")]
    [Range(0f, 1f)]
    [SerializeField] private float minAberration = 0.05f;

    [Range(0f, 1f)]
    [SerializeField] private float maxAberration = 0.7f;

    [Header("Granulado de pelicula")]
    [Range(0f, 1f)]
    [SerializeField] private float minGrain = 0.1f;

    [Range(0f, 1f)]
    [SerializeField] private float maxGrain = 0.75f;

    [Header("Transicion")]
    [SerializeField] private float smoothSpeed = 3f;

    private ChromaticAberration chromaticAberration;
    private FilmGrain filmGrain;

    private void Start()
    {
        if (player == null || globalVolume == null)
        {
            Debug.LogError("Faltan referencias en FuelPostProcessing.");
            enabled = false;
            return;
        }

        VolumeProfile profile = globalVolume.profile;

        if (!profile.TryGet(out chromaticAberration) ||
            !profile.TryGet(out filmGrain))
        {
            Debug.LogError(
                "Faltan Chromatic Aberration o Film Grain en el Volume."
            );
            enabled = false;
            return;
        }

        chromaticAberration.active = true;
        filmGrain.active = true;

        chromaticAberration.intensity.overrideState = true;
        filmGrain.intensity.overrideState = true;

        UpdateEffects(true);
    }

    private void Update()
    {
        UpdateEffects(false);
    }

    private void UpdateEffects(bool instant)
    {
        float fuel = Mathf.Clamp01(player.LanternCharge);

        // Cuanto menos combustible, mayor deterioro.
        float deterioration = 1f - fuel;

        // Intensifica los efectos cerca de quedarse sin gas.
        deterioration = Mathf.Pow(deterioration, 1.7f);

        float targetAberration = Mathf.Lerp(
            minAberration, maxAberration, deterioration
        );

        float targetGrain = Mathf.Lerp(
            minGrain, maxGrain, deterioration
        );

        float speed = instant
            ? 1f
            : 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);

        chromaticAberration.intensity.value = Mathf.Lerp(
            chromaticAberration.intensity.value,
            targetAberration,
            speed
        );

        filmGrain.intensity.value = Mathf.Lerp(
            filmGrain.intensity.value,
            targetGrain,
            speed
        );
    }
}
