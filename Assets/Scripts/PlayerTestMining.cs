using UnityEngine;

public class PlayerTestMining : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float moveSpeed;
    bool isMoving;
    float horizontalMovement;
    float verticalMovement;

    [SerializeField] float interactRange = 1.5f; // khoảng cách có thể đập quặng
    [SerializeField] LayerMask oreLayer; //Layer quặng
    [SerializeField] Camera mainCam; // camera chính (nếu để trống thì tự lấy)

    public bool IsMoving { get => isMoving; set => isMoving = value; }
    public float HorizontalMovement { get => horizontalMovement; }
    public float VerticalMovement { get => verticalMovement; }

    private void Awake()
    {
        if(mainCam == null)
            mainCam = Camera.main;
    }


    private void Update()
    {
        HandleMovement();
        HandleMining();
    } 
    void HandleMovement()
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
    void HandleMining()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryMineOre();
        }
    }
    void TryMineOre()
    {
        if (mainCam == null) return;

        // Lấy vị trí con trỏ chuột trong thế giới (world space)
        Vector3 mouseScreen = Input.mousePosition;
        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(mouseScreen);
        Vector2 mousePos2D = new Vector2(mouseWorld.x, mouseWorld.y);

        // Kiểm tra collider của quặng nằm tại vị trí chuột
        Collider2D hit = Physics2D.OverlapPoint(mousePos2D, oreLayer);

        if (hit != null)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist <= interactRange)
            {
                // Gọi script quặng để xử lý đào
                var ore = hit.GetComponent<Ore>(); // class quặng của bạn
                if (ore != null)
                {
                    ore.MineOre();
                }
            }
        }
    }    
}
