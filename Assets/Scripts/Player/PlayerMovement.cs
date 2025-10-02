using UnityEngine;

public class PlayerMovement:MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float moveSpeed;
    bool isMoving;
    float horizontalMovement;
    float verticalMovement;

    public bool IsMoving { get => isMoving; set => isMoving = value; }
    public float HorizontalMovement { get => horizontalMovement; }
    public float VerticalMovement { get => verticalMovement; }

    private void Update()
    {
        Vector2 direct = GameInput.Ins.GetInputMovementNormalize();
        
       
        isMoving = direct != Vector2.zero;
        if (isMoving)
        {
            horizontalMovement = direct.x;
            verticalMovement = direct.y;
        }
        rb.linearVelocity = direct * moveSpeed;
    }
}
