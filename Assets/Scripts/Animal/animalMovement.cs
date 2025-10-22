using UnityEngine;

public class animalMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 1f;
    private Vector2 movement;
    public Animator animator;
    public AnimalManager_Random manager;

    private float lifetime = 300f;

    // Vùng di chuyển ngẫu nhiên
    private Vector2 spawnAreaMin = new Vector2(-42f, -26f);
    private Vector2 spawnAreaMax = new Vector2(44.5f, 11.6f);

    private Vector2 currentTarget;
    private float waypointThreshold = 0.1f; 
    private float changeDirectionInterval = 5f; 
    private float directionTimer = 0f;
    private LayerMask farmlandMask;
    private LayerMask obstacleMask;

    void Start()
    {
        currentTarget = GetRandomPointInArea();
        farmlandMask = LayerMask.GetMask("Farmland");
        obstacleMask = LayerMask.GetMask("Obstacle");
    }

    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            if (manager != null)
            {
                manager.OnAnimalDied(gameObject);
            }
            Destroy(gameObject);
            return;
        }

        // Thay đổi hướng ngẫu nhiên sau mỗi khoảng thời gian
        directionTimer += Time.deltaTime;
        if (directionTimer >= changeDirectionInterval)
        {
            currentTarget = GetRandomPointInArea();
            directionTimer = 0f;
        }

        Vector2 direction = (currentTarget - rb.position).normalized;
        movement = direction;

        // Kiểm tra va chạm với farmland hoặc obstacle
        Vector2 nextPosition = rb.position + movement * speed * Time.deltaTime;
        if (Physics2D.OverlapCircle(nextPosition, 0.5f, farmlandMask) != null || Physics2D.OverlapCircle(nextPosition, 0.5f, obstacleMask) != null)
        {
            // Nếu va chạm, chọn hướng mới
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
}
