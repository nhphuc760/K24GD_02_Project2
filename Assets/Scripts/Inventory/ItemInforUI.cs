using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInforUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _itemName;
    [SerializeField] Image _icon;
    [SerializeField] TextMeshProUGUI _description;
    [SerializeField] Button _useButton;
    [SerializeField] Button _removeButton;
    [SerializeField] TMP_InputField _quantityInput;
    [SerializeField] GameObject descriptionUI;
    [SerializeField] GameObject removeItemUI;
    int slotIndex;

    public int SlotIndex { get => slotIndex; set => slotIndex = value; }

    private void Awake()
    {
        _removeButton.onClick.AddListener(RemoveItem);

    }

    private void OnDestroy()
    {
        _removeButton.onClick.RemoveAllListeners();
    }

    public void UpdateUI(InventorySlot inventorySlot)
    {
        ItemDataSO itemDataSO = inventorySlot.ItemData;
        _itemName.text = itemDataSO._itemName;
        _icon.sprite = itemDataSO._icon;
        if(itemDataSO.runTimeItemType.Equals(RunTimeItemType.None) || inventorySlot.dataRuntime == null)
        {
            _description.text = itemDataSO._description;
        }
        else
        {
            _description.text = itemDataSO._description + "\n" + inventorySlot.dataRuntime.ToString();
        }
           
    }

    

   public void RemoveItem()
    {
        int quantity;
        if(int.TryParse(_quantityInput.text, out quantity))
        {
            if(quantity > 0) {
                GameEventManager.Ins.inventoryEvent.RemoveItem(slotIndex, quantity);
                _quantityInput.text = "";
            }
        }
       
    }

    private void OnDisable()
    {
        descriptionUI.SetActive(true);
        removeItemUI.SetActive(false);
    }


    //Các hàm này gắn cho các button bật tắt lớp UI
    //dùng để đảm không nhận input game khi nhập dữ liệu
    public void Back()
    {
        GameEventManager.Ins.gameInput.Enable_InputAction();
    }
    public void Remove_StartBTN()
    {
        GameEventManager.Ins.gameInput.Disable_InputAction();
    }

}
