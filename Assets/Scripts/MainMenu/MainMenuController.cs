//quản lý việc chuyển đổi giữa các Scenes khác nhau khi người dùng tương tác với giao diện (UI).
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // tên scene có thể set ở Inspector nếu muốn
    public string playScene = "PlayScene";
    public string progressScene = "ProgressScene";
    public string shopScene = "ShopScene";
    public string settingScene = "SettingScene";

    public string play_G1Scene = "Play_G1Scene";
    public string play_G2Scene = "Play_G2Scene";
    public string play_G3Scene = "Play_G3Scene";


    // Hàm gọi khi nhấn màn Play
    public void OpenPlay()
    {
        SceneManager.LoadScene(playScene);
    }

    // Hàm gọi khi nhấn màn Progress
    public void OpenProgress()
    {
        SceneManager.LoadScene(progressScene);
    }

    // Hàm gọi khi nhấn màn
    public void OpenShop()
    {
        SceneManager.LoadScene(shopScene);
    }

    // Hàm gọi khi nhấn màn Settings
    public void OpenSettings()
    {
        SceneManager.LoadScene(settingScene);
    }
    // Hàm gọi khi nhấn màn
    public void OpenPlay_G1()
    {
        SceneManager.LoadScene(play_G1Scene);
    }
    public void OpenPlay_G2()
    {
        SceneManager.LoadScene(play_G2Scene);
    }
    public void OpenPlay_G3()
    {
        SceneManager.LoadScene(play_G3Scene);
    }

    
    // Hàm gọi khi nhấn nút Quit
    public void QuitGame()
    {
        // 
        Debug.Log("Quit requested");
        Application.Quit();
    }
}
