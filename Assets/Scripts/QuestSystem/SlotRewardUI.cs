using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotRewardUI : MonoBehaviour
{
    [SerializeField] Image _icon;
    [SerializeField] TextMeshProUGUI quantity;
    [SerializeField] Image backGround;
    public void UpdateUI(Sprite icon, int quantity, Sprite backGround)
    {
        this._icon.sprite = icon;
        this.quantity.text = quantity.ToString();
        this.backGround.sprite = backGround;
    }
}
