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
    private void Awake()
    {
        inventory = new Inventory(9, null); //Lấy tham chiếu tới 10 ô đầu tiên trong inventory
        
    }

    private void Start()
    {
        GameEventManager.Ins.toolKitEvent.onInitSuccess += InitInventorySuccess;
        GameEventManager.Ins.toolKitEvent.onCallInput += OnInputCalled;
        GameEventManager.Ins.toolKitEvent.onUpdateUI += UpdateUI;
        GameEventManager.Ins.toolKitEvent.onGetDataChooseSlot += GetCurDataChoosing;
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

    private void OnDestroy()
    {
        GameEventManager.Ins.toolKitEvent.onInitSuccess -= InitInventorySuccess;
        GameEventManager.Ins.toolKitEvent.onCallInput -= OnInputCalled;
        GameEventManager.Ins.toolKitEvent.onUpdateUI -= UpdateUI;
        GameEventManager.Ins.toolKitEvent.onGetDataChooseSlot -= GetCurDataChoosing;
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
    }
}
