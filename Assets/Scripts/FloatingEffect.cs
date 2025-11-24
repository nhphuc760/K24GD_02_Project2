using UnityEngine;

public class FloatingEffect : MonoBehaviour
{
    [Header("Cài đặt chuyển động")]
    [Tooltip("Khoảng cách di chuyển lên xuống (biên độ)")]
    public float amplitude = 10f;

    [Tooltip("Tốc độ di chuyển (tần số)")]
    public float frequency = 1f;

    // Lưu vị trí ban đầu để làm mốc
    private Vector3 startPos;

    void Start()
    {
        // Lưu lại vị trí ban đầu của object khi game bắt đầu
        startPos = transform.localPosition;
    }

    void Update()
    {
        // Tính toán vị trí mới dựa trên hàm Sin
        Vector3 tempPos = startPos;

        // Công thức: y = vị_trí_gốc + sin(thời_gian * tốc_độ) * khoảng_cách
        tempPos.y += Mathf.Sin(Time.time * Mathf.PI * frequency) * amplitude;

        // Cập nhật vị trí
        transform.localPosition = tempPos;
    }
}