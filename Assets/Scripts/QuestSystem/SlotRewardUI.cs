using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotRewardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image _icon;
    [SerializeField] TextMeshProUGUI quantity;
    [SerializeField] Image backGround;
    string _nameItem;
    RectTransform rectParent;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(_nameItem);
        Debug.Log(rectParent.name);
        GameEventManager.Ins.shopEvent.ShowToolTip(_nameItem, rectParent);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameEventManager.Ins.shopEvent.HideToolTip();
    }
    public void InitForToolTip(string _nameItem, RectTransform parent)
    {
        this._nameItem = _nameItem;
        this.rectParent = parent;
    }
    public void UpdateUI(Sprite icon, int quantity, Sprite backGround)
    {
        this._icon.sprite = icon;
        this.quantity.text = quantity.ToString();
        this.backGround.sprite = backGround;
    }

}
