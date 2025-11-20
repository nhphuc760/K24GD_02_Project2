using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropSlotUI : MonoBehaviour, IDragDrop
{
    [SerializeField] Image _icon;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI calMoney;
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI durability;
    [SerializeField] BlackSmith BlackSmith;
    [SerializeField] Button Repair;
    public InventorySlot curSlot;
    int curIndex = -1; 
    int durabilityOfTool;
    int spandDurability;
    int indexOfSlot;
    ToolDataSO curTool;
    public InventorySlot GetInventorySlot()
    {
        return curSlot;
    }

    private void Awake()
    {
       int count = Repair.onClick.GetPersistentEventCount();
        if(count == 0)
        {
            Repair.onClick.AddListener(RepairTool);
        }
    }

    private void OnEnable()
    {
        SetDefaultUI();

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
      
        //if (curSlot == null || curSlot.IsEmpty) return;
        //dragIcon = new GameObject("DragIcon").AddComponent<Image>();
        //dragIcon.sprite = curSlot.ItemData._icon;
        //dragIcon.color = Color.white;
        //dragIcon.raycastTarget = false;
        //dragIcon.transform.SetParent(transform.parent.parent);
        //dragIcon.rectTransform.sizeDelta = new Vector2(64, 64);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //if (curSlot == null || curSlot.IsEmpty) return;
        //if (dragIcon != null)
        //    dragIcon.transform.position = eventData.position;
    }

    public void OnDrop(PointerEventData eventData)
    {
        try
        {
            if (eventData.pointerDrag.TryGetComponent<IDragDrop>(out IDragDrop start) && eventData.pointerEnter.TryGetComponent<IDragDrop>(out IDragDrop end))
            {
                if (start.GetIndexSlot() == end.GetIndexSlot())
                {
                    return;
                }

                GameEventManager.Ins.inventoryEvent.DropItem(eventData);
                if (end.GetInventorySlot().IsEmpty)
                {
                    curSlot = null;
                    UpdateUI();
                }
            }
        }
        catch
        {

        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //if (dragIcon != null)
        //    Destroy(dragIcon.gameObject);
    }

    void UpdateUI()
    {
        if(curSlot == null || curSlot.IsEmpty)
        {
         _icon.enabled = false;
            title.enabled = true;
            calMoney.enabled = false;
        }
        else
        {
           _icon.enabled = true;
            _icon.sprite = curSlot.ItemData._icon;
            title.enabled = false;
        }
    }

    public void OnValueSliderChange(float value)
    {
        if (curSlot == null) return;
        int ceilValue = Mathf.CeilToInt(value);
        int valuee = Mathf.Max(ceilValue, durabilityOfTool);
        slider.value = valuee; 
        if(valuee == durabilityOfTool)
        {
            calMoney.enabled = false;
            Repair.interactable =false;
        }
        else
        {
            Repair.interactable = true;
            spandDurability = valuee - durabilityOfTool;
            calMoney.text = $"x{spandDurability}\n{curTool.repairCostPerPoint}$\n{spandDurability*curTool.repairCostPerPoint}$";
            calMoney.enabled = true;
        }
        durability.text = slider.value.ToString();
        
        

    }

    public void SetInventorySlot(InventorySlot slot)
    {

        if (slot == null || slot.IsEmpty)
        {
            curSlot = null;
            UpdateUI();

        }
        else 
        {
            ToolDataSO tool  = slot.ItemData as ToolDataSO;
            if (tool != null)
            {
                indexOfSlot = GameEventManager.Ins.inventoryEvent.GetIndexOfSlot(slot);
                curSlot = slot;
                curTool = tool;
                ToolRunTimeData dataRuntime = slot.dataRuntime as ToolRunTimeData;
                durabilityOfTool = dataRuntime != null ? dataRuntime.currentDurability : tool.maxDurability;
               slider.maxValue = tool.maxDurability;
                slider.minValue = 0;
                slider.value = dataRuntime.currentDurability;
                slider.interactable = true;
                UpdateUI();
            }
        }
      
    }

    public int GetIndexSlot()
    {
        return curIndex;
    }

    void SetDefaultUI()
    {
        _icon.enabled = false;
        title.enabled = true;
        calMoney.enabled = false;
        Repair.interactable = false;
        slider.interactable = false;
    }

    public async void RepairTool()
    {
        Debug.Log("RepairTool called: " + durabilityOfTool);
        Debug.Log("CurSlot: " + curSlot);
        if (curSlot == null || slider.value == durabilityOfTool) return;
        Debug.Log("Repair");
        if (BlackSmith.CheckCoin(curTool.repairCostPerPoint * spandDurability)) // curTool.repairCostPerPoint
        {
            BlackSmith.HideCanvas();
            ToolRunTimeData dataTool = curSlot.dataRuntime as ToolRunTimeData;
            float timeTrain = spandDurability * curTool.timePerpointRepair;
            await BlackSmith.StartTrain(timeTrain, curTool._icon);
            GameManager.Ins.Coin -= spandDurability * curTool.repairCostPerPoint;
            if (indexOfSlot >= 0)
                GameEventManager.Ins.inventoryEvent.RemoveItem(indexOfSlot, 1);
            else
                GameEventManager.Ins.inventoryEvent.RemoveItemByData(curTool, 1);
        }
    }
    public BlackSmithTrainData GetDataTool()
    {
        BlackSmithTrainData data = new BlackSmithTrainData
        {
            isTraing = BlackSmith.IsTraing,
            _idTool = curTool._id,
            _dataRuntime = curSlot.dataRuntime.SerializeData(),
            spandDurability = this.spandDurability,
            curDurability = durabilityOfTool,
            totalTime = spandDurability * curTool.timePerpointRepair
        };
        return data;
    }
}
