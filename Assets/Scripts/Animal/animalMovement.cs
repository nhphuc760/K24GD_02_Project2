using UnityEngine;
using System.Collections.Generic;

public class animalMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 1f;
    private Vector2 movement;
    public Animator animator;
    public AnimalManager_Random manager;
    public List<GameObject> dropItems; 

    private float lifetime = 100f;

    private Vector2 spawnAreaMin = new Vector2(-42f, -26f);
    private Vector2 spawnAreaMax = new Vector2(44.5f, 11.6f);

    private Vector2 currentTarget;
    private float waypointThreshold = 0.1f; 
    private float changeDirectionInterval = 5f; 
    private float directionTimer = 0f;
    private LayerMask farmlandMask;
    private LayerMask obstacleMask;
    private LayerMask waterMask;
    private bool hasHorizontalParam = false;
    private bool hasVerticalParam = false;
    private bool hasSpeedParam = false;

    void Start()
    {
        currentTarget = GetRandomPointInArea();
        farmlandMask = LayerMask.GetMask("Farmland");
        obstacleMask = LayerMask.GetMask("Obstacle");
        waterMask = LayerMask.GetMask("Water");

        if (animator == null)
        {
            Debug.LogWarning("animalMovement.Start: Animator reference is null. Please assign an Animator in the inspector.");
        }
        else
        {
            // Cache which parameters exist to avoid runtime errors when calling SetFloat
            var parameters = animator.parameters;
            foreach (var p in parameters)
            {
                if (p.name == "Horizontal") hasHorizontalParam = true;
                if (p.name == "Vertical") hasVerticalParam = true;
                if (p.name == "Speed") hasSpeedParam = true;
            }

            if (!hasHorizontalParam) Debug.LogWarning("animalMovement.Start: Animator is missing parameter 'Horizontal'.");
            if (!hasVerticalParam) Debug.LogWarning("animalMovement.Start: Animator is missing parameter 'Vertical'.");
            if (!hasSpeedParam) Debug.LogWarning("animalMovement.Start: Animator is missing parameter 'Speed'.");
        }
    }

    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            DropItem();
            if (manager != null)
            {
                manager.OnAnimalDied(gameObject);
            }
            Destroy(gameObject);
            return;
        }

        directionTimer += Time.deltaTime;
        if (directionTimer >= changeDirectionInterval || Vector2.Distance(rb.position, currentTarget) < waypointThreshold)
        {
            currentTarget = GetRandomPointInArea();
            directionTimer = 0f;
        }

        Vector2 direction = (currentTarget - rb.position).normalized;
        movement = direction;

        Vector2 nextPosition = rb.position + movement * speed * Time.deltaTime;
        if (Physics2D.OverlapCircle(nextPosition, 0.5f, farmlandMask) != null || Physics2D.OverlapCircle(nextPosition, 0.5f, obstacleMask) != null || Physics2D.OverlapCircle(nextPosition, 0.5f, waterMask) != null)
        {
            currentTarget = GetRandomPointInArea();
            direction = (currentTarget - rb.position).normalized;
            movement = direction;
        }

        if (animator != null)
        {
            if (hasHorizontalParam) animator.SetFloat("Horizontal", movement.x);
            if (hasVerticalParam) animator.SetFloat("Vertical", movement.y);
            if (hasSpeedParam) animator.SetFloat("Speed", movement.sqrMagnitude);
        }
    }
    
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    private Vector2 GetRandomPointInArea()
    {
        float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        return new Vector2(x, y);
    }

    private void DropItem()
    {
        if (dropItems == null || dropItems.Count == 0)
        {
            Debug.LogWarning("animalMovement.DropItem: dropItems list is empty or null. Assign prefabs in inspector.");
            return;
        }

        int randomIndex = Random.Range(0, dropItems.Count);
        GameObject prefab = dropItems[randomIndex];
        if (prefab == null)
        {
            Debug.LogWarning($"animalMovement.DropItem: Selected dropItems[{randomIndex}] is null.");
            return;
        }

        // Small random offset so the spawned object isn't exactly overlapping the animal
        Vector3 spawnPos = transform.position + new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.1f, 0.3f), 0f);
        GameObject droppedItem = Instantiate(prefab, spawnPos, Quaternion.identity);
        droppedItem.transform.SetParent(null);

        // Ensure visibility on top of the scene (2D sprite) if a SpriteRenderer exists
        var sr = droppedItem.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            // bump sorting order so it is likely visible above other sprites
            sr.sortingOrder = Mathf.Max(sr.sortingOrder, 50);
        }

        // Ensure it has a Rigidbody2D so physics/pickup systems can interact with it
        var itemRb = droppedItem.GetComponent<Rigidbody2D>();
        if (itemRb == null)
        {
            itemRb = droppedItem.AddComponent<Rigidbody2D>();
        }
        itemRb.gravityScale = 0.5f;
        itemRb.linearVelocity = Vector2.zero;
        itemRb.AddForce(new Vector2(Random.Range(-20f, 20f), Random.Range(30f, 60f)));

        // Attach or ensure a DroppedItem script exists for pickup logic
        DroppedItem droppedItemScript = droppedItem.GetComponent<DroppedItem>();
        if (droppedItemScript == null)
        {
            droppedItemScript = droppedItem.AddComponent<DroppedItem>();
            Debug.Log("animalMovement.DropItem: Added DroppedItem component to instantiated prefab.");
        }

        Debug.Log($"animalMovement.DropItem: Spawned '{droppedItem.name}' at {spawnPos} (prefab index {randomIndex}).");
    }
}
