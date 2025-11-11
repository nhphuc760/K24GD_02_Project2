using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    //public Transform CheckGroundPoint;
    //private Tilemap[] _groundTilemaps;
    bool isMoving;
    float horizontalMovement;
    float verticalMovement;
    //private bool _isGrounded;
    //public bool IsGrounded
    //{
    //    get => _isGrounded;
    //    private set
    //    {
    //        _isGrounded = value;
    //        //Upgrade later: Change animation state if have swimming or flying
    //    }
    //}
    public bool IsMoving => isMoving;
    public float HorizontalMovement { get => horizontalMovement; }
    public float VerticalMovement { get => verticalMovement; }

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //var playerFarming = GetComponent<PlayerFarming>();
        //if (playerFarming.enabled == false) return;
        //Tilemap[] allTilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        //_groundTilemaps = System.Array.FindAll(allTilemaps, tm => tm.gameObject.layer == LayerMask.NameToLayer("Ground"));
    }

    void Update()
    {
        Vector2 direct = GameEventManager.Ins.gameInput.GetInputMovementNormalize();
        isMoving = direct != Vector2.zero;
        if (isMoving)
        {
            horizontalMovement = direct.x;
            verticalMovement = direct.y;
        }
        rb.linearVelocity = direct * moveSpeed;
    }

    //private bool CheckOnGround(Vector2 direction)
    //{
    //    if (CheckGroundPoint == null)
    //    {
    //        return false;
    //    }
    //    Vector3 nextPos = CheckGroundPoint.transform.position + (Vector3)(direction * 0.1f);
    //    Vector3Int cellPos;
    //    foreach (Tilemap tilemap in _groundTilemaps)
    //    {
    //        cellPos = tilemap.WorldToCell(nextPos);
    //        if (tilemap.GetTile(cellPos) != null)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}
    public Vector2 GetDirection()
    {
        return new Vector2 (horizontalMovement, verticalMovement);
    }
}
