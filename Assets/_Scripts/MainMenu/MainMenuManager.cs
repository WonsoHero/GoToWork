using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject SettingPanel;

    public void OnStartBtnClicked(string sceneName)
    {
       SceneManager.LoadScene(sceneName);
    }
    public void OnSettingBtnClicked()
    {
        ShowSettingPanel();
    }
    public void OnExitBtnClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    public void ShowSettingPanel()
    {
        SettingPanel.SetActive(true);
    }

    public void HideSettingPanel()
    {
        SettingPanel.SetActive(false);
    }
}
