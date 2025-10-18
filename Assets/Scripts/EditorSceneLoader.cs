// EditorSceneLoader.cs
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR // Dòng này đảm bảo script chỉ hoạt động trong Unity Editor

public static class EditorSceneLoader
{
    // Đặt tên scene "bất tử" của bạn ở đây
    private static string persistentSceneName = "PersistentSystem";

    // Thuộc tính này ra lệnh cho Unity chạy hàm này tự động KHI NHẤN NÚT PLAY
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void LoadPersistentSystems()
    {
        // Nếu chúng ta đã ở trong scene "bất tử" rồi, thì không làm gì cả
        if (SceneManager.GetActiveScene().name == persistentSceneName)
        {
            return;
        }

        // Kiểm tra xem scene "bất tử" đã được tải hay chưa
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == persistentSceneName)
            {
                return;
            }
        }

        // Nếu chưa, hãy tải nó "âm thầm" vào cùng lúc
        Debug.LogWarning($"--- [EditorSceneLoader] Tự động tải {persistentSceneName} ---");
        SceneManager.LoadScene(persistentSceneName, LoadSceneMode.Additive);
    }
}

#endif