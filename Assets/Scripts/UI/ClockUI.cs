using UnityEngine;
using UnityEngine.UI;

public class ClockUI : MonoBehaviour
{
    [Header("Components")]
    public RectTransform clockHandTransform; // Kéo cái kim vào đây

    [Header("Settings")]
    // Offset góc quay nếu kim mặc định không chỉ 12h (ví dụ kim vẽ ngang thì offset là 90)
    public float rotationOffset = 0f;

    /// <summary>
    /// Hàm này sẽ được UIManager gọi mỗi khi thời gian thay đổi.
    /// </summary>
    public void UpdateTime(int hour, int minute)
    {
        if (clockHandTransform == null) return;

        // 1. Tính tổng số phút đã trôi qua trong ngày (0 -> 1440)
        float totalMinutes = (hour * 60) + minute;

        // 2. Tính góc quay:
        // 1 ngày (1440 phút) = 360 độ
        // => 1 phút = 0.25 độ
        // Dấu trừ (-) để quay theo chiều kim đồng hồ
        float rotationZ = -totalMinutes * 0.25f;

        // 3. Áp dụng góc quay (cộng thêm offset nếu cần)
        clockHandTransform.localRotation = Quaternion.Euler(0, 0, rotationZ + rotationOffset);
    }
}