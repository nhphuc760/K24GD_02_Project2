using UnityEngine;

/// <summary>
/// Attach to fruit prefab. Allows player to pull the fruit when near or when pressing a key.
/// Assumes the player GameObject is tagged "Player".
/// When collected the fruit returns itself to FruitPool (if exists) or is destroyed.
/// </summary>
public class FruitPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [Tooltip("Distance within which player can press the pickup key to pull the fruit")]
    public float pickupRadius = 2f;

    [Tooltip("Key used to start pulling the fruit")]
    public KeyCode pickupKey = KeyCode.E;

    [Tooltip("Speed at which the fruit moves towards the player when pulled")]
    public float pullSpeed = 8f;

    [Tooltip("If fruit gets closer than this distance, it will be considered collected")]
    public float collectDistance = 0.3f;

    [Tooltip("If true, fruit will be auto-collected when it is already very close to the player")]
    public bool autoCollectWhenClose = true;

    private Transform player;
    private bool isPulling = false;

    void Start()
    {
        var p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null)
        {
            var p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (!isPulling)
        {
            if (autoCollectWhenClose && dist <= collectDistance)
            {
                Collect();
                return;
            }

            if (dist <= pickupRadius && Input.GetKeyDown(pickupKey))
            {
                isPulling = true;
            }
        }

        if (isPulling)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, pullSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, player.position) <= collectDistance)
            {
                Collect();
            }
        }
    }

    /// <summary>
    /// External callers can start pulling explicitly (e.g., UI action).
    /// </summary>
    public void StartPull(Transform playerTransform, float speedOverride = 0f)
    {
        player = playerTransform;
        isPulling = true;
        if (speedOverride > 0f) pullSpeed = speedOverride;
    }

    private void Collect()
    {
        // TODO: add give-to-player logic (inventory, score, sound, etc.)

        if (FruitPool.Instance != null)
        {
            FruitPool.Instance.ReturnToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
