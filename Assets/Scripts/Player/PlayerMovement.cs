using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement:MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float moveSpeed;
    [SerializeField] float swimSpeed;
    public Transform CheckGroundPoint;
    private Tilemap[] _groundTilemaps;
    bool isMoving;
    bool isFishing;
    float horizontalMovement;
    float verticalMovement;
    private bool _isGrounded;
    public bool IsGrounded
    {
        get => _isGrounded;
        private set
        {
            _isGrounded = value;
            //Upgrade later: Change animation state if have swimming or flying
        }
    }
    public bool IsFishing { get => isFishing; set => isFishing = value; }
    public bool IsMoving { get => isMoving; set => isMoving = value; }
    public float HorizontalMovement { get => horizontalMovement; }
    public float VerticalMovement { get => verticalMovement; }
    private void Awake()
    {
        Tilemap[] allTilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        _groundTilemaps = System.Array.FindAll(allTilemaps, tm => tm.gameObject.layer == LayerMask.NameToLayer("Ground"));
    }

    private void Update()
    {
        
    }
    private void FixedUpdate()
    {
        Vector2 direct = GameInput.Ins.GetInputMovementNormalize();

        isMoving = direct != Vector2.zero;
        if (isMoving && !isFishing)
        {
            horizontalMovement = direct.x;
            verticalMovement = direct.y;
        }
        IsGrounded = CheckOnGround(direct);
        if (IsGrounded)
        {
            rb.linearVelocity = direct * moveSpeed;
        }
        else
        {
            rb.linearVelocity = direct * swimSpeed;
        }
    }
    private bool CheckOnGround(Vector2 direction)
    {
        Vector3 nextPos = CheckGroundPoint.transform.position + (Vector3)(direction * 0.1f);
        Vector3Int cellPos;
        foreach (Tilemap tilemap in _groundTilemaps)
        {
            cellPos = tilemap.WorldToCell(nextPos);
            if (tilemap.GetTile(cellPos) != null)
            {
                return true;
            }
        }
        return false;
    }
}
