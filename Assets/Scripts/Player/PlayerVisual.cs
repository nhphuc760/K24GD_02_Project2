using System;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PlayerVisual : MonoBehaviour
{

    [SerializeField] PlayerFishing player;
    [SerializeField] public Animator animator;
    [SerializeField] PlayerMovement playerMovement;

  
    bool isInteract = false;
    private void Awake()
    {
        GameManager.Ins.onLoadDataCompleted += LoadDataPlayerCompleted; 
        player ??= transform.parent.GetComponent<PlayerFishing>();
        
    }

    private void Start()
    {
        GameEventManager.Ins.animationEvent.onToolUse += UseTool;
        GameEventManager.Ins.animationEvent.onDead += Dead;
        GameEventManager.Ins.animationEvent.onIdle += Idle;
    }

    private void Idle()
    {
        animator.Play(AnimationHashes.IDLE);
    }

    private void UseTool(IToolTarget target, ToolDataSO sO, ToolRunTimeData dataRunTime)
    {
        if (!isInteract)
        {
            isInteract = true;
            animator.Play(target.RequireTool.ToString());
            StartCoroutine(WaitUseTool(target, sO, dataRunTime));
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

    public bool winnerAnimIsActive;
    public AnimationCurve animationCurve;

    private void Update()
    {
        animator.SetBool(AnimationHashes.ISMOVING, playerMovement.IsMoving);
        animator.SetFloat(AnimationHashes.HORIZONTAL_MOVEMENT, playerMovement.HorizontalMovement);
        animator.SetFloat(AnimationHashes.VERTICAL_MOVEMENT, playerMovement.VerticalMovement);
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
    private void OnDestroy()
    {
        if (GameEventManager.Ins != null)
        {
            GameManager.Ins.onLoadDataCompleted -= LoadDataPlayerCompleted;
            GameEventManager.Ins.animationEvent.onToolUse -= UseTool;
            GameEventManager.Ins.animationEvent.onDead -= Dead;
            GameEventManager.Ins.animationEvent.onIdle -= Idle;
        }
    }

    IEnumerator WaitUseTool(IToolTarget target, ToolDataSO sO, ToolRunTimeData tool)
    {
        yield return null;
        float duration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duration);
        target.InteractWithTool(sO, tool);
        isInteract = false; 
    }

    public void OnCastFishingEndAnimationEvent()
    {
        player.OnCastFishingEnd();
    }
    public void OnCaptureFishStartAnimationEvent()
    {
        player.CheckIfWinFishGame();
    }
    public void Dead()
    {
        animator.Play(AnimationHashes.DEAD);
    }
}

