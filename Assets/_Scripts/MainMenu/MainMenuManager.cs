using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private void Update()
    {
        
    }
    public void OnStartBtnClicked(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void OnSettingBtnClicked()
    {

    }
    public void OnExitBtnClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
