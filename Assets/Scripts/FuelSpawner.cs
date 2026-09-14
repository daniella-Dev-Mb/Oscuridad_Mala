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
    [Min(0.1f)] public float failedPlacementRetryDelay = 2f;
    public LayerMask obstacleLayers = Physics.DefaultRaycastLayers;
    [Tooltip("Contorno jugable en coordenadas mundiales X/Z. Vacio usa todo el terreno.")]
    public Vector2[] spawnBoundary = new Vector2[0];

    FuelPickup[] pickups;
    float[] timers;
    Vector2Int cells;
    TerrainCollider groundCollider;
    Vector2 areaMin;
    Vector2 areaMax;

    void Start()
    {
        if (terrain == null || terrain.terrainData == null || pickupPrefab == null)
        {
            Debug.LogWarning("Asignar el terreno y el prefab de combustible al spawner.", this);
            enabled = false;
            return;
        }

        cells = new Vector2Int(Mathf.Max(1, grid.x), Mathf.Max(1, grid.y));
        groundCollider = terrain.GetComponent<TerrainCollider>();
        if (groundCollider == null || !groundCollider.enabled)
        {
            Debug.LogError("El generador necesita un TerrainCollider activo.", this);
            enabled = false;
            return;
        }
        Vector3 terrainPosition = terrain.transform.position;
        Vector3 size = terrain.terrainData.size;
        areaMin = new Vector2(terrainPosition.x + edgeMargin, terrainPosition.z + edgeMargin);
        areaMax = new Vector2(terrainPosition.x + size.x - edgeMargin, terrainPosition.z + size.z - edgeMargin);
        if (spawnBoundary.Length >= 3)
        {
            Vector2 min = spawnBoundary[0], max = min;
            foreach (Vector2 point in spawnBoundary)
            {
                min = Vector2.Min(min, point);
                max = Vector2.Max(max, point);
            }
            areaMin = Vector2.Max(areaMin, min);
            areaMax = Vector2.Min(areaMax, max);
        }
        if (areaMax.x <= areaMin.x || areaMax.y <= areaMin.y)
        {
            Debug.LogError("El contorno de combustible no intersecta el terreno.", this);
            enabled = false;
            return;
        }
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
        timers[index] = Mathf.Max(0.1f, failedPlacementRetryDelay);
        TerrainData data = terrain.terrainData;
        Vector3 size = data.size;
        Vector2 cellSize = new Vector2((areaMax.x - areaMin.x) / cells.x,
            (areaMax.y - areaMin.y) / cells.y);

        for (int attempt = 0; attempt < placementAttempts; attempt++)
        {
            float x = areaMin.x + (index % cells.x + Random.value) * cellSize.x;
            float z = areaMin.y + (index / cells.x + Random.value) * cellSize.y;
            if (!ContainsPoint(new Vector2(x, z))) continue;
            if (data.GetSteepness((x - terrain.transform.position.x) / size.x,
                (z - terrain.transform.position.z) / size.z) > maxSlope)
            {
                continue;
            }

            // Consultar el collider asignado evita rechazos por terrenos superpuestos.
            Vector3 rayOrigin = new Vector3(x, terrain.transform.position.y + size.y + 10f, z);
            if (!groundCollider.Raycast(new Ray(rayOrigin, Vector3.down), out RaycastHit hit, size.y + 20f))
                continue;
            Vector3 position = hit.point + Vector3.up * groundOffset;
            Vector3 probe = position + Vector3.up * clearanceRadius;
            if (IsBlocked(probe))
            {
                continue;
            }

            pickups[index] = Instantiate(pickupPrefab, position, Quaternion.identity, transform);
            timers[index] = Mathf.Max(0f, respawnDelay);
            return;
        }
    }

    public bool ContainsPoint(Vector2 point)
    {
        if (spawnBoundary.Length < 3) return true;
        bool inside = false;
        for (int i = 0, j = spawnBoundary.Length - 1; i < spawnBoundary.Length; j = i++)
        {
            Vector2 a = spawnBoundary[i], b = spawnBoundary[j];
            if ((a.y > point.y) != (b.y > point.y) &&
                point.x < (b.x - a.x) * (point.y - a.y) / (b.y - a.y) + a.x)
                inside = !inside;
        }
        return inside;
    }

    void OnDrawGizmosSelected()
    {
        if (spawnBoundary.Length < 3) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < spawnBoundary.Length; i++)
        {
            Vector2 a = spawnBoundary[i], b = spawnBoundary[(i + 1) % spawnBoundary.Length];
            Vector3 start = new Vector3(a.x, transform.position.y + 0.2f, a.y);
            Vector3 end = new Vector3(b.x, transform.position.y + 0.2f, b.y);
            Gizmos.DrawLine(start, end);
        }
    }

    bool IsBlocked(Vector3 position)
    {
        Collider[] obstacles = Physics.OverlapSphere(position, clearanceRadius,
            obstacleLayers, QueryTriggerInteraction.Ignore);
        foreach (Collider obstacle in obstacles)
        {
            // La pendiente del suelo no es un obstaculo para apoyar la botella.
            if (obstacle is TerrainCollider) continue;
            return true;
        }
        return false;
    }
}
