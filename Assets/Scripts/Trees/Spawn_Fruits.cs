using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Fruits : MonoBehaviour
{
    [Header("Prefabs & Spawn Points")]
    [Tooltip("One or more fruit prefabs to spawn")]
    public List<GameObject> fruitPrefabs = new List<GameObject>();

    [Tooltip("Transforms where fruits can be spawned. If empty, fruits will be spawned at this GameObject's position.")]
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("Timing")]
    [Tooltip("Seconds between each spawn")]
    public float spawnInterval = 10f;

    [Header("Optional Spawn Area (rectangle)")]
    [Tooltip("If true, fruits will spawn at a random position inside the rectangle defined by the four corners (world coordinates). Corners can be provided in any order.")]
    public bool useSpawnRectangle = false;

    [Tooltip("If true, the rectangle corners are interpreted as local coordinates relative to this GameObject. If false, corners are world coordinates.")]
    public bool rectIsLocal = true;

    [Tooltip("If true, the rectangle defined by the corners will be centered on the tree's position. Useful to make the rectangle span all sides around the tree.")]
    public bool centerRectOnTree = true;

    [Tooltip("Corner A of the spawn rectangle (world coords)")]
    public Vector3 rectCornerA = new Vector3(-25f, -2f, 0f);
    [Tooltip("Corner B of the spawn rectangle (world coords)")]
    public Vector3 rectCornerB = new Vector3(-25f, -16f, 0f);
    [Tooltip("Corner C of the spawn rectangle (world coords)")]
    public Vector3 rectCornerC = new Vector3(0f, -16f, 0f);
    [Tooltip("Corner D of the spawn rectangle (world coords)")]
    public Vector3 rectCornerD = new Vector3(0f, 0f, 0f);

    [Header("Spawn distance rules")]
    [Tooltip("Minimum distance from the tree's position that a spawn must be. The spawner will try to resample up to maxSpawnAttempts times.")]
    public float minDistanceFromTree = 1f;

    [Tooltip("Maximum attempts to find a spawn point that satisfies minDistanceFromTree. If exceeded, the last sampled position is used.")]
    public int maxSpawnAttempts = 10;

    private Coroutine spawnCoroutine;

    void Start()
    {
        // Start the repeating spawn coroutine
        if (spawnInterval <= 0f) spawnInterval = 10f;
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    void OnDisable()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
    }

    IEnumerator SpawnRoutine()
    {
        // Wait a short moment before first spawn so the scene can initialize if needed
        yield return new WaitForSeconds(0.1f);

        while (true)
        {
            SpawnOneFruit();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnOneFruit()
    {
        if (fruitPrefabs == null || fruitPrefabs.Count == 0)
        {
            Debug.LogWarning("Spawn_Fruits: No fruitPrefabs assigned.");
            return;
        }

        // pick a prefab
        var prefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Count)];
        if (prefab == null)
        {
            Debug.LogWarning("Spawn_Fruits: Selected prefab is null.");
            return;
        }

        // determine spawn position
        Vector3 pos;
        Quaternion rot = Quaternion.identity;

        if (useSpawnRectangle)
        {
            // compute axis-aligned bounds from the four corners (order doesn't matter)
            // corners can be local or world depending on rectIsLocal
            Vector3 a = rectCornerA;
            Vector3 b = rectCornerB;
            Vector3 c = rectCornerC;
            Vector3 d = rectCornerD;

            if (rectIsLocal)
            {
                a = transform.TransformPoint(rectCornerA);
                b = transform.TransformPoint(rectCornerB);
                c = transform.TransformPoint(rectCornerC);
                d = transform.TransformPoint(rectCornerD);
            }

            Vector3 min = Vector3.Min(Vector3.Min(a, b), Vector3.Min(c, d));
            Vector3 max = Vector3.Max(Vector3.Max(a, b), Vector3.Max(c, d));

            if (centerRectOnTree)
            {
                // shift rectangle so its center is at the tree position
                Vector3 rectCenter = (min + max) * 0.5f;
                Vector3 offset = transform.position - rectCenter;
                min += offset;
                max += offset;
                a += offset; b += offset; c += offset; d += offset;
            }

            // attempt to sample a position that is at least minDistanceFromTree away from the tree
            Vector3 sample = Vector3.zero;
            bool found = false;
            for (int attempt = 0; attempt < Mathf.Max(1, maxSpawnAttempts); attempt++)
            {
                float rx = Random.Range(min.x, max.x);
                float ry = Random.Range(min.y, max.y);
                float rz = Random.Range(min.z, max.z);
                sample = new Vector3(rx, ry, rz);
                if (Vector3.Distance(sample, transform.position) >= minDistanceFromTree)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                // last-resort: if nothing satisfies min distance, use last sample but log a message
                Debug.LogWarning("Spawn_Fruits: Could not find spawn position satisfying minDistanceFromTree after attempts; using last sample.");
            }

            pos = sample;
            rot = Quaternion.identity;
        }
        else if (spawnPoints != null && spawnPoints.Count > 0)
        {
            var sp = spawnPoints[Random.Range(0, spawnPoints.Count)];
            if (sp != null)
            {
                pos = sp.position;
                rot = sp.rotation;
            }
            else
            {
                pos = transform.position;
            }
        }
        else
        {
            pos = transform.position;
        }

            GameObject go = null;
            // Prefer using the pool if available
            if (FruitPool.Instance != null)
            {
                go = FruitPool.Instance.Spawn(prefab, pos, rot);
            }
            else
            {
                go = Instantiate(prefab, pos, rot);
            }

            // make spawned fruit a child of this tree for scene organization
            if (go != null)
                go.transform.SetParent(this.transform, true);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        try
        {
            Gizmos.color = Color.yellow;
            if (spawnPoints != null)
            {
                foreach (var sp in spawnPoints)
                {
                    if (sp == null) continue;
                    Gizmos.DrawSphere(sp.position, 0.05f);
                }
            }

            if (useSpawnRectangle)
            {
                Gizmos.color = Color.cyan;
                // draw rectangle edges using the four corners
                Vector3 a = rectCornerA;
                Vector3 b = rectCornerB;
                Vector3 c = rectCornerC;
                Vector3 d = rectCornerD;

                if (rectIsLocal && transform != null)
                {
                    a = transform.TransformPoint(rectCornerA);
                    b = transform.TransformPoint(rectCornerB);
                    c = transform.TransformPoint(rectCornerC);
                    d = transform.TransformPoint(rectCornerD);
                }

                if (centerRectOnTree && transform != null)
                {
                    Vector3 min = Vector3.Min(Vector3.Min(a, b), Vector3.Min(c, d));
                    Vector3 max = Vector3.Max(Vector3.Max(a, b), Vector3.Max(c, d));
                    Vector3 rectCenter = (min + max) * 0.5f;
                    Vector3 offset = transform.position - rectCenter;
                    a += offset; b += offset; c += offset; d += offset;
                }

                Gizmos.DrawLine(a, b);
                Gizmos.DrawLine(b, c);
                Gizmos.DrawLine(c, d);
                Gizmos.DrawLine(d, a);

                // draw corners
                Gizmos.DrawSphere(a, 0.06f);
                Gizmos.DrawSphere(b, 0.06f);
                Gizmos.DrawSphere(c, 0.06f);
                Gizmos.DrawSphere(d, 0.06f);
            }
        }
        catch (System.Exception ex)
        {
            // Catch any editor-time exceptions coming from gizmos drawing so they don't spam console.
            Debug.LogException(ex, this);
        }
    }
#endif
}
