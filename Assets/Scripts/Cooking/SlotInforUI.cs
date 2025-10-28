using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotInforUI : MonoBehaviour
{
    [SerializeField] KitchenInventoryManager kitchenInventoryManager;
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] Image _icon;
    [SerializeField] Button cook;
    [SerializeField] Transform ingredientContainer;
    KitchenItemDataSO curkitchenItemSO;
    void Start()
    {
        cook.onClick.AddListener(() => { 
            GameEventManager.Ins.cookingEvent.OnCookClick(curkitchenItemSO.recipeSO); 
        });
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        cook.onClick.RemoveAllListeners();
    }
    private void OnDisable()
    {
        curkitchenItemSO = null;
    }

    public void PointerClickSlotUI(int index)
    {
          
        if(index >= kitchenInventoryManager.kitchenItemList.Count)
        {
            Debug.Log(index + "out of range kitchenItemList");
            return;
        }
        KitchenItemDataSO kitchenItem = kitchenInventoryManager.kitchenItemList[index];
        if(kitchenItem != curkitchenItemSO)
        {
            curkitchenItemSO = kitchenItem;
            UpdateUI();
        }
        Debug.Log("SetActiveSlotInfor true");
        gameObject.SetActive(true);
    }

    void UpdateUI()
    {
        if (curkitchenItemSO == null) return;
        _nameText.text = curkitchenItemSO._itemName;
        _icon.sprite = curkitchenItemSO._icon;
        description.text = curkitchenItemSO._description;
        ClearAllChildrent(ingredientContainer);
        foreach (var child in curkitchenItemSO.recipeSO.ingredients)
        {
            int curQty = GameEventManager.Ins.cookingEvent.GetTotalItem(child.itemSO);
            string text = "";
            if(curQty >= child.quantity)
            {
                text = $"<color=green>{curQty}/{child.quantity}</color>";
            }
            else
            {
                text = $"<color=red>{curQty}/{child.quantity}</color>";
            }
                InstantiateIngredientItem(child.itemSO._icon,text);
        }
    }

    public void ClearAllChildrent(Transform parent)
    {
       //for(int i = 0; i< parent.childCount; i++)
       // {
       //     Destroy(parent.GetChild(i));
       // }
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }

    }

    void InstantiateIngredientItem(Sprite icon, string text)
    {
        // Tạo GameObject chứa image (parent)
        GameObject itemGO = new GameObject("IngredientItemUI", typeof(RectTransform), typeof(Image));
        Image itemReward = itemGO.GetComponent<Image>();
        itemReward.sprite = icon;
        RectTransform itemRect = itemGO.GetComponent<RectTransform>();
        // thiết kích thước mặc định cho parent (nếu muốn thay đổi, set khác)
        itemRect.sizeDelta = new Vector2(120, 120);
        itemRect.localScale = Vector3.one;

        // Tạo GameObject con chứa TextMeshProUGUI bằng AddComponent
        GameObject qtyGO = new GameObject("Quantity", typeof(RectTransform));
        qtyGO.transform.SetParent(itemGO.transform, false);
        TextMeshProUGUI quantity = qtyGO.AddComponent<TextMeshProUGUI>();

        // Lấy kích thước parent từ itemRect.sizeDelta (đã đặt phía trên)
        float parentWidth = itemRect.sizeDelta.x;
        float parentHeight = itemRect.sizeDelta.y;
        if (parentWidth <= 0) parentWidth = 120f;
        if (parentHeight <= 0) parentHeight = 120f;

        // Cấu hình rect transform cho text:
        // - Đính vào Bottom Center của parent (anchor và pivot ở bottom center)
        // - Width = parentWidth, Height = 1/4 parentHeight
        RectTransform qtyRect = quantity.rectTransform;
        qtyRect.SetParent(itemGO.transform, false);
        qtyRect.anchorMin = new Vector2(0.5f, 0f);
        qtyRect.anchorMax = new Vector2(0.5f, 0f);
        qtyRect.pivot = new Vector2(0.5f, 0f); // bottom center pivot
        qtyRect.anchoredPosition = Vector2.zero; // bottom center
        qtyRect.sizeDelta = new Vector2(parentWidth, parentHeight * 0.33f);
        qtyRect.localScale = Vector3.one;

        // Cấu hình text: Middle Left alignment nhưng nằm ở bottom-center của parent

        quantity.text = text;
        quantity.fontStyle = FontStyles.Bold;
        quantity.alignment = TextAlignmentOptions.Midline;
        quantity.color = Color.black;
        quantity.enableAutoSizing = true;
        quantity.fontSizeMin = 6;
        quantity.fontSizeMax = 40;
        quantity.raycastTarget = false;

        // Parent vào container chính (không thay đổi transform local)
        itemGO.transform.SetParent(ingredientContainer, false);
    }
}
