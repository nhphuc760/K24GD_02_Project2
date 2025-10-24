using UnityEngine;
using UnityEngine.SceneManagement;

public class Store_Button_Manager : MonoBehaviour
{
    public void OnExitButtonClicked()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentIndex > 0)
        {
            SceneManager.LoadScene(currentIndex - 1);
        }
        else
        {
            Debug.Log("Không có scene trước để quay về.");
        }
    }
}
