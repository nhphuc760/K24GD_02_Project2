using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInforUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _itemName;
    [SerializeField] Image _icon;
    [SerializeField] TextMeshProUGUI _description;
    [SerializeField] Button _useButton;
    [SerializeField] Button _confirmButton;
    [SerializeField] TMP_InputField _quantityInput;
    [SerializeField] GameObject descriptionUI;
    [SerializeField] GameObject removeItemUI;
    [SerializeField] Sprite _backGroundSell;
    [SerializeField] Button _removeButton;
    [SerializeField] TextMeshProUGUI textButtonSell;
    int slotIndex;
    bool isSell;
    Color colorDefault;
    Sprite spriteDefault;
    [SerializeField] InventoryManager inventoryManager;
    public int SlotIndex { get => slotIndex; set => slotIndex = value; }

    private void Awake()
    {
        colorDefault = _removeButton.image.color;
        spriteDefault = _removeButton.image.sprite;
        GameEventManager.Ins.onNearSell += OnNearSell;
        _confirmButton.onClick.AddListener(RemoveItem);

    }

    private void OnNearSell(bool obj)
    {
        isSell = obj;
    }

    private void OnEnable()
    {
        if (isSell)
        {
            _removeButton.image.sprite = _backGroundSell;
           _removeButton.image.color = Color.white;
            textButtonSell.text = "Bán";
        }
        else
        {
            _removeButton.image.sprite = spriteDefault;
            _removeButton.image.color = colorDefault;
            textButtonSell.text = "Loại bỏ";
        }
    }
    private void OnDestroy()
    {
        _confirmButton.onClick.RemoveAllListeners();
        GameEventManager.Ins.onNearSell -= OnNearSell;
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
                if (isSell)
                { 
                    ItemDataSO dataSO  = inventoryManager.inventory.itemSlots[slotIndex].ItemData;
                    if (dataSO.isCanSell)
                    {
                        int total = dataSO.sell * quantity;
                        GameManager.Ins.Coin += total;
                        GameEventManager.Ins.TriggerDialog($"<color=green>+{total}</color>"); 
                    }
                    else
                    {
                        GameEventManager.Ins.TriggerDialog("<color=red>Vật phẩm không thể bán</color>");
                        return;
                    }
                }
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
