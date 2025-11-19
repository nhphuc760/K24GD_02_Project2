using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolKit : MonoBehaviour
{
    [SerializeField] ToolKitSlotUI toolkitSlotUI;
    public Inventory inventory;
    List<ToolKitSlotUI> cachedToolkitUI = new();
    [SerializeField] Transform container;
    InventorySlot curChoosing = null;
    int curIndex = -1;
    [SerializeField] Animator animator;

    int OPEN = Animator.StringToHash("ToolKitOpen");
    int CLOSED = Animator.StringToHash("ToolKitClosed");
    bool isOpen = true;
    private void Awake()
    {
        inventory = new Inventory(9, null); //Lấy tham chiếu tới 10 ô đầu tiên trong inventory
        animator ??= GetComponent<Animator>();
    }

    private void Start()
    {
        GameEventManager.Ins.toolKitEvent.onInitSuccess += InitInventorySuccess;
    }

    private void OnEnable()
    {
     
        GameEventManager.Ins.toolKitEvent.onCallInput += OnInputCalled;
        GameEventManager.Ins.toolKitEvent.onUpdateUI += UpdateUI;
        GameEventManager.Ins.toolKitEvent.onGetDataChooseSlot += GetCurDataChoosing;
        GameEventManager.Ins.toolKitEvent.onGetCurrentIndexChoose += GetCurIndexChoosing;
        GameEventManager.Ins.toolKitEvent.onTrigger += Trigger;
    }

    private int GetCurIndexChoosing()
    {
        return curIndex;
    }

    private InventorySlot GetCurDataChoosing()
    {
        return curChoosing;
    }

    private void OnInputCalled(int input)//input là các số từ 1-9 nhưng mảng thì lưu tử 0-8
    {
        Debug.Log("Input toolKit: " + input);
      OnPointerClickSlotUI(input - 1);
    }

    private void OnDisable()
    {
      
        GameEventManager.Ins.toolKitEvent.onCallInput -= OnInputCalled;
        GameEventManager.Ins.toolKitEvent.onUpdateUI -= UpdateUI;
        GameEventManager.Ins.toolKitEvent.onGetDataChooseSlot -= GetCurDataChoosing;
        GameEventManager.Ins.toolKitEvent.onGetCurrentIndexChoose -= GetCurIndexChoosing;
        GameEventManager.Ins.toolKitEvent.onTrigger -= Trigger;
    }

    private void InitInventorySuccess(Inventory inventory)
    {

        // Mỗi lần khởi tạo sẽ lấy 9 phần tử đầu tiên trong inventory.
        for (int i = 0; i < 9; i++)
        {
            this.inventory.itemSlots[i] = inventory.itemSlots[i];
            var slot = Instantiate(toolkitSlotUI, container);
            slot.Init(i, this);
            cachedToolkitUI.Add(slot);
            slot.UpdateUI();
        }
        OnInputCalled(1); //Mặc định vào game sẽ chọn ô toolkit đầu tiên
        GameEventManager.Ins.toolKitEvent.onInitSuccess -= InitInventorySuccess;
    }

    void UpdateUI()
    {
        foreach (var child in cachedToolkitUI)
        {
            child.UpdateUI();
        }
    }
    public void OnPointerClickSlotUI(int newIndex)
    {
        Debug.Log("Pointer Click: " + newIndex);
        if (curIndex == newIndex) return;
       if(curIndex >= 0 && curIndex< 9)
        cachedToolkitUI[curIndex].DisableSelected();
       if(newIndex >= 0 && newIndex < 9)
        cachedToolkitUI[newIndex].EnableSelected();
        curIndex = newIndex;
        curChoosing = inventory.itemSlots[curIndex];
        GameEventManager.Ins.toolKitEvent.curSelectedChange(curChoosing);
    }
    

   void Trigger()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            animator.Play(OPEN);
        }
        else
        {
            animator.Play(CLOSED);
        }
    }
}
