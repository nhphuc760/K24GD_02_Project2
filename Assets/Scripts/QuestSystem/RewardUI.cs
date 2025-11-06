using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RewardUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI buffReward;
    [SerializeField] Transform containerItem;
    
    public void UpdateUI(RewardData rewardData)
    {
        if (buffReward != null)
        {
            buffReward.fontStyle = FontStyles.Italic;
            buffReward.color = Color.black;
            buffReward.text = $"Exp: {rewardData.experience}, Gold: {rewardData.gold}";
        }

        if (containerItem == null)
        {
            Debug.LogError("[RewardUI] containerItem is not assigned in the Inspector.");
            return;
        }

        if (rewardData.items == null) return;
        foreach (var item in rewardData.items)
        {
            if (item.itemSO != null) 
                InstantiateItemReward(item.itemSO._icon, item.quantity);
        }
    }


    void InstantiateItemReward(Sprite icon, int _quantity)
    {
        // Tạo GameObject chứa image (parent)
        GameObject itemGO = new GameObject("ItemUI", typeof(RectTransform), typeof(Image));
        Image itemReward = itemGO.GetComponent<Image>();
        itemReward.sprite = icon;
        RectTransform itemRect = itemGO.GetComponent<RectTransform>();
        // thiết kích thước mặc định cho parent (nếu muốn thay đổi, set khác)
        itemRect.sizeDelta = new Vector2(70, 70);
        itemRect.localScale = Vector3.one;

        // Tạo GameObject con chứa TextMeshProUGUI bằng AddComponent
        GameObject qtyGO = new GameObject("Quantity", typeof(RectTransform));
        qtyGO.transform.SetParent(itemGO.transform, false);
        TextMeshProUGUI quantity = qtyGO.AddComponent<TextMeshProUGUI>();

        // Lấy kích thước parent từ itemRect.sizeDelta (đã đặt phía trên)
        float parentWidth = itemRect.sizeDelta.x;
        float parentHeight = itemRect.sizeDelta.y;
        if (parentWidth <= 0) parentWidth = 70f;
        if (parentHeight <= 0) parentHeight = 70f;

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
        
        quantity.text = _quantity.ToString();
        quantity.fontStyle = FontStyles.Bold;
        quantity.alignment = TextAlignmentOptions.MidlineLeft;
        quantity.color = Color.black;
        quantity.enableAutoSizing = true;
        quantity.fontSizeMin = 6;
        quantity.fontSizeMax = 40;
        quantity.raycastTarget = false;

        // Parent vào container chính (không thay đổi transform local)
        itemGO.transform.SetParent(containerItem, false);
    }

}
