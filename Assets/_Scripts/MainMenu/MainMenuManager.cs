using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject SettingPanel;

    public async void OnStartBtnClicked(string sceneName)
    {
        await SceneManager.LoadSceneAsync(sceneName);
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
