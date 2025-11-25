using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RequireItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image _icon;
    [SerializeField] TextMeshProUGUI _inforHas;
    string _name;
    RectTransform _parent;
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameEventManager.Ins.shopEvent.ShowToolTip(_name, _parent);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameEventManager.Ins.shopEvent.HideToolTip();
    }

    public void UpdateUI(Sprite _icon, string _inforHas)
    {
        this._icon.sprite = _icon;
        this._inforHas.text = _inforHas;
    }
    
    public void SetItemName(string name)
    {
        this._name = name;  
    }
    public void Init(string name, RectTransform parent)
    {
        this._name = name;
        this._parent = parent;
    }

}
