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
    public void UpdateUI(ItemDataSO itemDataSO)
    {
        _itemName.text = itemDataSO._itemName;
        _icon.sprite = itemDataSO._icon;
        _description.text = itemDataSO._description;
    }
}
