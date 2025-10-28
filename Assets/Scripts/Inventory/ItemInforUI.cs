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

    public void UpdateUI(ItemDataSO itemDataSO)
    {
        _itemName.text = itemDataSO._itemName;
        _icon.sprite = itemDataSO._icon;
        _description.text = itemDataSO._description;
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

}
