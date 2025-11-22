using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerTestMining : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float interactRange = 1.5f; // khoảng cách có thể đập quặng
    [SerializeField] LayerMask oreLayer; //Layer quặng
    [SerializeField] Transform groundCheck;
    [SerializeField] PlayerMovement playerMovement;
    ToolDataSO currentTool;
    InventorySlot curToolKit;

    private void Awake()
    {
        if (playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement>();
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnSceneLoad(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name.Equals("MiningScene") || arg0.name.Equals("ForestScene"))
        {
            this.enabled = true;
        }
        else
        {
            this.enabled = false;
        }
    }


    private void OnEnable()
    {
        GameEventManager.Ins.gameInput.interacPressed += TryMineOre;
        GameEventManager.Ins.toolKitEvent.onCurSelectedChange += CurToolSelectedChanged;
    }

    private void CurToolSelectedChanged(InventorySlot obj)
    {
        curToolKit = obj;
        currentTool = curToolKit?.ItemData as ToolDataSO;
    }

    private void OnDisable()
    {
        GameEventManager.Ins.gameInput.interacPressed -= TryMineOre;
        GameEventManager.Ins.toolKitEvent.onCurSelectedChange -= CurToolSelectedChanged;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    void TryMineOre()
    {

        if (playerMovement.IsMoving) return;
        if (currentTool == null)
        {
            GameEventManager.Ins.TriggerDialog("<color=red>Bạn chưa chọn công cụ</color>");
            return;
        }
        Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, interactRange, oreLayer);
        if (hit != null)
        {

            // Gọi script quặng để xử lý đào
            if (hit.TryGetComponent<IToolTarget>(out IToolTarget toolTarget) && CheckDirection(hit.transform))
            {
                if (currentTool.toolType.Equals(toolTarget.RequireTool))
                {
                    GameEventManager.Ins.animationEvent.ToolUse(toolTarget, currentTool, curToolKit.dataRuntime as ToolRunTimeData);
                }
                else
                    GameEventManager.Ins.TriggerDialog("<color=red>Công cụ không phù hợp để khai thác</color>");
            }

        }
    }


    bool CheckDirection(Transform obj)
    {
        if (playerMovement == null) return false;
        Vector2 direct = (obj.position - groundCheck.position).normalized;
        float t = Vector2.Dot(direct, playerMovement.GetDirection());
        if (t > 0)
        {
            return true;
        }
        else if (t < 0)
        {
            return false;
        }
        return false;
    }


    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(groundCheck.position, Vector3.forward, interactRange);
    }
}
