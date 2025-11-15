using Assets.Scripts.Player;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    public static PlayerVisual Ins {get; private set; }

    [SerializeField] public Animator animator;
    [SerializeField] PlayerMovement playerMovement;
    public bool winnerAnimIsActive;
    public AnimationCurve animationCurve;

    // Animator Parametter Hashes
    public static int ISMOVING = Animator.StringToHash("isMoving");
    public static int HORIZONTAL_MOVEMENT = Animator.StringToHash("horizontalMovement");
    public static int VERTICAL_MOVEMENT = Animator.StringToHash("verticalMovement");

    private void Awake()
    {
        if (Ins != null && Ins != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Ins = this;
    }
    private void Update()
    {
        animator.SetBool(ISMOVING, playerMovement.IsMoving);
        animator.SetFloat(HORIZONTAL_MOVEMENT, playerMovement.HorizontalMovement);
        animator.SetFloat(VERTICAL_MOVEMENT, playerMovement.VerticalMovement);
        if (playerMovement.IsMoving)
        {
            FlipVisual();
        }
        
    }

    void FlipVisual() 
    {
        if (playerMovement.HorizontalMovement < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (playerMovement.HorizontalMovement > 0)
        {
            transform.localScale = Vector3.one;
        }
    }
    public void OnCastFishingEndAnimationEvent()
    {
        Player.Ins.OnCastFishingEnd();
    }
    public void OnCaptureFishStartAnimationEvent()
    {
        Player.Ins.CheckIfWinFishGame();
    }
}

