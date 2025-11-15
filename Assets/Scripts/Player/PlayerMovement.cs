using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    bool isMoving;
    float horizontalMovement;
    float verticalMovement;
    public bool IsMoving => isMoving;
    public float HorizontalMovement { get => horizontalMovement; }
    public float VerticalMovement { get => verticalMovement; }

    private Rigidbody2D rb;
    Vector2 direct;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
         direct = GameEventManager.Ins.gameInput.GetInputMovementNormalize();
        isMoving = direct != Vector2.zero;
      
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            horizontalMovement = direct.x;
            verticalMovement = direct.y;
        }
        rb.linearVelocity = direct * moveSpeed;
    }

    public Vector2 GetDirection()
    {
        return new Vector2 (horizontalMovement, verticalMovement);
    }
}
