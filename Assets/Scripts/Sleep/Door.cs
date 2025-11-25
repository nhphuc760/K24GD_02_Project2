using UnityEngine;

// Kế thừa từ IInteractable để Player có thể nhấn nút tương tác
public class Door : MonoBehaviour, IInteractable
{
    [Header("Destination")]
    [Tooltip("Tên scene bên trong nhà (ví dụ: 'HouseInside')")]
    public string sceneToLoad;

    [Tooltip("Vị trí người chơi sẽ xuất hiện bên trong nhà")]
    public Vector3 targetPosition;

    // Hàm này được gọi khi Player đứng gần và nhấn Space/E
    public void Interact()
    {
        Debug.Log($"Mở cửa vào nhà: {sceneToLoad}");

        // Gọi GameManager để chuyển cảnh (giống hệt Portal)
        if (GameManager.instance != null)
        {
            GameManager.instance.StartSceneTransition(sceneToLoad, targetPosition);
        }
    }
}