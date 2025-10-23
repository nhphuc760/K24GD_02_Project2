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

    void Start()
    {
        currentTarget = GetRandomPointInArea();
        farmlandMask = LayerMask.GetMask("Farmland");
        obstacleMask = LayerMask.GetMask("Obstacle");
        waterMask = LayerMask.GetMask("Water");
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

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
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
        if (dropItems != null && dropItems.Count > 0)
        {
            int randomIndex = Random.Range(0, dropItems.Count);
            GameObject droppedItem = Instantiate(dropItems[randomIndex], transform.position, Quaternion.identity);
            
            // Thêm component DroppedItem nếu chưa có
            DroppedItem droppedItemScript = droppedItem.GetComponent<DroppedItem>();
            if (droppedItemScript == null)
            {
                droppedItemScript = droppedItem.AddComponent<DroppedItem>();
            }
            // Giả sử dropItems có ItemDataSO, hoặc set mặc định
            // droppedItemScript.itemData = ...; // Cần set ItemDataSO tương ứng
        }
    }
}
