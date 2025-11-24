using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; 
using TMPro;          

public class UIHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Cài đặt hiệu ứng")]
    public Color glowColor = new Color(1f, 0.9f, 0.6f, 1f); // Vàng sáng
    public bool enableScale = true;
    public float scaleAmount = 1.1f;

    // Biến lưu trữ component (có thể là TMP hoặc Text thường)
    private TextMeshProUGUI tmpText;
    private Text legacyText;

    private Color originalColor;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;

        // 1. Thử tìm TextMeshPro trước
        tmpText = GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText != null)
        {
            originalColor = tmpText.color;
            return; // Tìm thấy rồi thì thôi
        }

        // 2. Nếu không thấy TMP, thử tìm Text thường (Legacy)
        legacyText = GetComponentInChildren<Text>();
        if (legacyText != null)
        {
            originalColor = legacyText.color;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Đổi màu (kiểm tra xem đang dùng loại text nào)
        if (tmpText != null) tmpText.color = glowColor;
        else if (legacyText != null) legacyText.color = glowColor;

        // Phóng to
        if (enableScale) transform.localScale = originalScale * scaleAmount;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Trả về màu cũ
        if (tmpText != null) tmpText.color = originalColor;
        else if (legacyText != null) legacyText.color = originalColor;

        // Trả về kích thước cũ
        if (enableScale) transform.localScale = originalScale;
    }
}