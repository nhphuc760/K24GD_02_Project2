using System;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] PlayerMovement playerMovement;
    static int ISMOVING = Animator.StringToHash("isMoving");
    static int HORIZONTAL_MOVEMENT = Animator.StringToHash("horizontalMovement");
    static int VERTICAL_MOVEMENT = Animator.StringToHash("verticalMovement");
    bool isPickAxe = false;

    private void Awake()
    {
        GameManager.Ins.onLoadDataCompleted += LoadDataPlayerCompleted;
    }

    private void Start()
    {
        GameEventManager.Ins.animationEvent.onPickAxe += AnimationEvent_onPickAxe;
    }

    private void AnimationEvent_onPickAxe(OreInfor obj)
    {
        if(!isPickAxe)
        {
            isPickAxe = true;
            animator.Play("PickAxe_BlendTree");
            StartCoroutine(MineOre(obj));
        }
      
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
        GameEventManager.Ins.animationEvent.onPickAxe -= AnimationEvent_onPickAxe;
    }

    IEnumerator MineOre(OreInfor ore)
    {
        yield return null;
        float duration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duration);
        ore.MineOre();
        isPickAxe = false;
    }
}
