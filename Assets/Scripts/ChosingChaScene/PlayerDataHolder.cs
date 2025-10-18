using UnityEngine;
using UnityEngine.U2D.Animation;

public class PlayerDataHolder : MonoBehaviour
{
    public static PlayerDataHolder instance;

    public string selectedCharacterName;
    public Sprite selectedPlayerAvatar;

    public SpriteLibraryAsset selectedPlayerSpriteLibrary;//lưu thư viện sprite của nhân vật đã chọn

    private void Awake()
    {
        Debug.Log("<color=green>[AWAKE] PlayerDataHolder đang chạy!</color>");
        // Singleton Pattern: Đảm bảo chỉ có duy nhất 1 "người đưa thư" tồn tại
        if (instance == null)
        {
            instance = this;
            // Thẻ VIP: "Đừng phá hủy tôi khi chuyển cảnh!"
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Nếu đã có một người đưa thư rồi, hãy tự hủy bản sao này
            Destroy(gameObject);
        }
    }
}
