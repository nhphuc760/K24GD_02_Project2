using UnityEngine;
using UnityEngine.U2D.Animation;

public class PlayerCharacterChanger : MonoBehaviour
{
    private SpriteLibrary playerSpriteLibrary;
    private SpriteResolver playerSpriteResolver;

    void Awake()
    {
        // Cache các component một lần duy nhất
        playerSpriteLibrary = GetComponentInChildren<SpriteLibrary>();
        playerSpriteResolver = GetComponentInChildren<SpriteResolver>();
    }

    private void Start()
    {
        // Kịch bản Game Mới: Tự chạy nếu PlayerDataHolder có dữ liệu
        if (PlayerDataHolder.instance != null && !string.IsNullOrWhiteSpace(PlayerDataHolder.instance.selectedCharacterName))
        {
            Debug.Log("--- PlayerCharacterChanger: Chạy chế độ Game Mới ---");
            // Gọi chính hàm public bên dưới để áp dụng dữ liệu
            ApplyCharacterData(PlayerDataHolder.instance.selectedPlayerSpriteLibrary);
        }
        // Nếu không có dữ liệu (chạy test), hàm Start sẽ không làm gì cả
        // và đợi lệnh từ GameManager.
    }

    /// <summary>
    /// Hàm public này được GameManager gọi trong Test Mode,
    /// hoặc được chính hàm Start() gọi trong New Game Mode.
    /// </summary>
    public void ApplyCharacterData(SpriteLibraryAsset libAsset)
    {
        // Kiểm tra lại cho chắc chắn
        if (playerSpriteLibrary == null)
        {
            playerSpriteLibrary = GetComponentInChildren<SpriteLibrary>();
        }

        if (playerSpriteLibrary != null && libAsset != null)
        {
            playerSpriteLibrary.spriteLibraryAsset = libAsset;
            Debug.Log("Đã áp dụng SpriteLibrary cho người chơi.");

            // Set resolver về trạng thái mặc định
            if (playerSpriteResolver == null)
            {
                playerSpriteResolver = GetComponentInChildren<SpriteResolver>();
            }
            playerSpriteResolver.SetCategoryAndLabel("Idle_Front", "Idle_0"); // Đặt lại trạng thái
        }
        else
        {
            Debug.LogError("Áp dụng SpriteLibrary thất bại! (Asset hoặc Library bị null)");
        }
    }
}