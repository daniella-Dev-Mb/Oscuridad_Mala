using UnityEngine;

public class FuelSpawner : MonoBehaviour
{
    public Terrain terrain;
    public FuelPickup pickupPrefab;
    public Vector2Int grid = new Vector2Int(10, 10);
    [Min(0f)] public float edgeMargin = 5f;
    [Min(0f)] public float respawnDelay = 45f;
    [Range(0f, 90f)] public float maxSlope = 30f;
    [Min(0.01f)] public float clearanceRadius = 0.75f;
    [Min(0f)] public float groundOffset = 0.05f;
    [Min(1)] public int placementAttempts = 12;
    public LayerMask obstacleLayers = Physics.DefaultRaycastLayers;

    FuelPickup[] pickups;
    float[] timers;
    Vector2Int cells;

    void Start()
    {
        if (terrain == null || terrain.terrainData == null || pickupPrefab == null)
        {
            Debug.LogWarning("Asignar el terreno y el prefab de combustible al spawner.", this);
            enabled = false;
            return;
        }

        cells = new Vector2Int(Mathf.Max(1, grid.x), Mathf.Max(1, grid.y));
        pickups = new FuelPickup[cells.x * cells.y];
        timers = new float[pickups.Length];
        Physics.SyncTransforms();
        for (int i = 0; i < pickups.Length; i++)
        {
            Spawn(i);
        }
    }

    void Update()
    {
        for (int i = 0; i < pickups.Length; i++)
        {
            if (pickups[i] != null)
            {
                timers[i] = respawnDelay;
                continue;
            }

            timers[i] -= Time.deltaTime;
            if (timers[i] <= 0f && Time.deltaTime > 0f)
            {
                Spawn(i);
            }
        }
    }

    void Spawn(int index)
    {
        timers[index] = Mathf.Max(0f, respawnDelay);
        TerrainData data = terrain.terrainData;
        Vector3 size = data.size;
        float margin = Mathf.Clamp(edgeMargin, 0f, Mathf.Min(size.x, size.z) * 0.5f);
        Vector2 cellSize = new Vector2((size.x - margin * 2f) / cells.x,
            (size.z - margin * 2f) / cells.y);

        for (int attempt = 0; attempt < placementAttempts; attempt++)
        {
            float x = margin + (index % cells.x + Random.value) * cellSize.x;
            float z = margin + (index / cells.x + Random.value) * cellSize.y;
            if (data.GetSteepness(x / size.x, z / size.z) > maxSlope)
            {
                continue;
            }

            Vector3 position = terrain.transform.position + new Vector3(x, 0f, z);
            position.y += terrain.SampleHeight(position) + groundOffset;
            // Rechaza huecos del terreno, arboles y otros obstaculos.
            Vector3 probe = position + Vector3.up * clearanceRadius;
            if (!Physics.Raycast(probe, Vector3.down, out RaycastHit hit,
                    clearanceRadius + groundOffset, Physics.DefaultRaycastLayers,
                    QueryTriggerInteraction.Ignore) || hit.collider.gameObject != terrain.gameObject
                || Physics.CheckSphere(probe, clearanceRadius, obstacleLayers, QueryTriggerInteraction.Ignore))
            {
                continue;
            }

            pickups[index] = Instantiate(pickupPrefab, position, Quaternion.identity, transform);
            return;
        }
    }
}
