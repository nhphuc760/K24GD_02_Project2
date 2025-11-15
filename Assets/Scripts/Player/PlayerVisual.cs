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
    bool isInteract = false;
    private void Awake()
    {
        GameManager.Ins.onLoadDataCompleted += LoadDataPlayerCompleted;
    }

    private void Start()
    {
        GameEventManager.Ins.animationEvent.onToolUse += UseTool;
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
        GameEventManager.Ins.animationEvent.onToolUse -= UseTool;
    }

    IEnumerator WaitUseTool(IToolTarget target, ToolDataSO sO, ToolRunTimeData tool)
    {
        yield return null;
        float duration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duration);
        target.InteractWithTool(sO, tool);
        isInteract = false;
    }
}
