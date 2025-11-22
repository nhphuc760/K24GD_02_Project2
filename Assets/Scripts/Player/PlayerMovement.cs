
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    bool isMoving;
    float horizontalMovement;
    float verticalMovement;
    public bool IsMoving => isMoving;
    public float HorizontalMovement { get => horizontalMovement; }
    public float VerticalMovement { get => verticalMovement; }
    Vector2 direct;
    private Rigidbody2D rb;


    //them am thanh foot step o day 
    [SerializeField] AudioClip[] footstepSounds; 
    [SerializeField] float footstepInterval = 0.4f; 
    private float footstepTimer; 
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
         direct = GameEventManager.Ins.gameInput.GetInputMovementNormalize();
        isMoving = direct != Vector2.zero;

        HandleFootsteps();

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

    //footstep handler
    void HandleFootsteps()
    {
        // Chỉ xử lý khi nhân vật đang di chuyển
        if (isMoving)
        {
            // Đếm ngược thời gian
            footstepTimer -= Time.deltaTime;

            // Khi hết giờ đếm ngược
            if (footstepTimer <= 0)
            {
                int index = GetIndex();
                AudioManager.instance.PlayFX(footstepSounds[index]); ;

                // Reset lại đồng hồ
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            // Khi đứng yên, reset timer về 0.
            // Để ngay khi bắt đầu đi lại, tiếng bước chân sẽ phát ngay lập tức (cảm giác nhạy hơn).
            footstepTimer = 0;
        }
    }

    int GetIndex()
    {
        return UnityEngine.Random.Range(0, footstepSounds.Length);
    }
}
