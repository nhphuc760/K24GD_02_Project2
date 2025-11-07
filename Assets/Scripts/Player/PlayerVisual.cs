using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] PlayerMovement playerMovement;
    static int ISMOVING = Animator.StringToHash("isMoving");
    static int HORIZONTAL_MOVEMENT = Animator.StringToHash("horizontalMovement");
    static int VERTICAL_MOVEMENT = Animator.StringToHash("verticalMovement");



    private void Awake()
    {
        GameManager.Ins.onLoadDataCompleted += LoadDataPlayerCompleted;
    }

    private void LoadDataPlayerCompleted(PlayerData data, CharacterDataSO sO)
    {
       if(sO == null)
        {
            Debug.Log("From PlayerVisual: CharacterDataSO is null" );
            return;
        }
        GetComponent<SpriteLibrary>().spriteLibraryAsset = sO.SpriteLibraryAsset;
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

    void FlipVisual() {
        if (playerMovement.HorizontalMovement < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (playerMovement.HorizontalMovement > 0)
        {
            transform.localScale = Vector3.one;
        }
    }
    private void OnDestroy()
    {
        GameManager.Ins.onLoadDataCompleted -= LoadDataPlayerCompleted;
    }
}
