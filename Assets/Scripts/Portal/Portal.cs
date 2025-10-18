using UnityEngine;

public class Portal : MonoBehaviour
{
    public string sceneToLoad; // Tên của cảnh sẽ được tải khi người chơi tương tác với cổng
    public Vector3 targetPosition; // Vị trí mục tiêu trong cảnh mới


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player đã chạm vào cổng, chuẩn bị tải scene: {sceneToLoad}");

            // Ra lệnh cho GameManager "bất tử" xử lý việc chuyển cảnh
            if (GameManager.instance != null)
            {
                GameManager.instance.StartSceneTransition(sceneToLoad, targetPosition);
            }
        }
    }
}
