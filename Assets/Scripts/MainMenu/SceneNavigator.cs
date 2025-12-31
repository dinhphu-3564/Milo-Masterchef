//Chức năng nút Back (Quay lại Menu chính)

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    // Chuyển đến scene được chỉ định
    public void GoToScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
            SceneManager.LoadScene(sceneName);
        else
            Debug.LogWarning("SceneNavigator.GoToScene called with empty sceneName");
    }

    // Chuyển về Menu chính
    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
