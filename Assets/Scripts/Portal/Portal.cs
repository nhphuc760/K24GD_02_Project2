using System.Threading.Tasks;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public string sceneToLoad; // Tên của cảnh sẽ được tải khi người chơi tương tác với cổng
    public Vector3 targetPosition; // Vị trí mục tiêu trong cảnh mới


    private  async void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player đã chạm vào cổng, chuẩn bị tải scene: {sceneToLoad}");

            // Ra lệnh cho GameManager "bất tử" xử lý việc chuyển cảnh
            if (GameManager.Ins != null)
            {
                await GameManager.Ins.StartSceneTransition(sceneToLoad, targetPosition);
            }
            if (Physics2D.GetIgnoreLayerCollision(0, 2))
            {
                Physics2D.IgnoreLayerCollision(0, 2, false);
                SpriteRenderer playerVisual = other.gameObject.GetComponentInChildren<SpriteRenderer>();
                if (playerVisual != null)
                {
                    playerVisual.sortingLayerName = "Player";
                    playerVisual.sortingOrder = 0;
                }
                
            }
        }
    }
}
