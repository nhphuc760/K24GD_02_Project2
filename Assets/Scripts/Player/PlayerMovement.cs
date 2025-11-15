using Assets.Scripts;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement:MonoBehaviour
{
    public static PlayerMovement Ins { get; private set; }
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float moveSpeed;
    [SerializeField] float swimSpeed;
    public Transform CheckGroundPoint;
    private Tilemap[] _groundTilemaps;
    bool isMoving;
    float horizontalMovement;
    float verticalMovement;
    private bool _isGrounded;
    public FacingDirection facingDirection { get; private set; } = FacingDirection.Right;
    public bool IsGrounded
    {
        get => _isGrounded;
        private set
        {
            _isGrounded = value;
            //Upgrade later: Change animation state if have swimming or flying
        }
    }
    public bool IsMoving { get => isMoving; set => isMoving = value; }
    public float HorizontalMovement { get => horizontalMovement; }
    public float VerticalMovement { get => verticalMovement; }
    private void Awake()
    {
        if (Ins != null && Ins != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Ins = this;
        DontDestroyOnLoad(this.gameObject);
        Tilemap[] allTilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        _groundTilemaps = System.Array.FindAll(allTilemaps, tm => tm.gameObject.layer == LayerMask.NameToLayer("Ground"));
    }

    private void Update()
    {
        
    }
    private void FixedUpdate()
    {
        Vector2 direct = GameInput.Ins.GetInputMovementNormalize();
        if (PauseController.IsGamePaused || Player.Ins.isInFishingState)
        {
            rb.linearVelocity = Vector2.zero;
            direct = Vector2.zero;
            return;
        }
        isMoving = direct != Vector2.zero;
        if (isMoving)
        {
            SoundEffectManager.Play("StepOnDirt");
            horizontalMovement = direct.x;
            verticalMovement = direct.y;
            UpdateFacingDirection(horizontalMovement, verticalMovement);
        }
        IsGrounded = CheckOnGround(direct);
        if (IsGrounded)
        {
            rb.linearVelocity = direct * moveSpeed;
        }
        else if (!IsGrounded)
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
    private void UpdateFacingDirection(float x, float y)
    {
        if (Mathf.Abs(x) > Mathf.Abs(y))
            facingDirection = (x > 0) ? FacingDirection.Right : FacingDirection.Left;
        else if (Mathf.Abs(y) > 0)
            facingDirection = (y > 0) ? FacingDirection.Back : FacingDirection.Front;
    }
}
