using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float swimSpeed;
    public Transform CheckGroundPoint;
    private Tilemap[] _groundTilemaps;
    bool isMoving;
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
    public bool IsMoving { get => isMoving; set => isMoving = value; }
    public float HorizontalMovement { get => horizontalMovement; }
    public float VerticalMovement { get => verticalMovement; }
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (SceneManager.GetActiveScene().name.Equals("Farm")) return;
        Tilemap[] allTilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        _groundTilemaps = System.Array.FindAll(allTilemaps, tm => tm.gameObject.layer == LayerMask.NameToLayer("Ground"));
    }

    void Update()
    {
        // Use old Input system for simplicity
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 direct = new Vector2(h, v).normalized;
        isMoving = direct != Vector2.zero;
        if (isMoving)
        {
            horizontalMovement = direct.x;
            verticalMovement = direct.y;
        }
        rb.linearVelocity = direct * moveSpeed;
        //IsGrounded = CheckOnGround(direct);
        //if (IsGrounded)
        //{
        //    rb.linearVelocity = direct * moveSpeed;
        //}
        //else
        //{
        //    rb.linearVelocity = direct * swimSpeed;
        //}
    }

    private bool CheckOnGround(Vector2 direction)
    {
        if (CheckGroundPoint == null)
        {
            return false;
        }
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
    }
}
