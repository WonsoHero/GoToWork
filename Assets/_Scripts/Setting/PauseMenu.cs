using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    //[SerializeField] GameObject SoundSettingCanvas;
    [SerializeField] GameObject PauseMenuCanvas;

    private static PauseMenu instance;
    public static PauseMenu Instance
    {
        get { return instance; }
    }

    public bool PauseMenuActiveSelf { get { return pauseMenuActiveSelf; } }
    private bool pauseMenuActiveSelf = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnSoundSettingPressed()
    {
        //if (SoundSettingCanvas != null)
        //{
        //    SoundSettingCanvas.SetActive(true);
        //}
    }

    public void ShowPauseMenu()
    {
        Debug.Log("ShowPauseMenu");
        Time.timeScale = 0f;
        PauseMenuCanvas?.SetActive(true);
        pauseMenuActiveSelf = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HidePauseMenu()
    {
        Time.timeScale = 1f;
        PauseMenuCanvas?.SetActive(false);
        pauseMenuActiveSelf = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnLastCheckPointPressed()
    {
        HidePauseMenu();
        // 임시
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public async void OnMainMenuPressed()
    {
        Time.timeScale = 1f;
        PauseMenuCanvas?.SetActive(false);
        pauseMenuActiveSelf = false;
        await SceneManager.LoadSceneAsync("MainMenu");
    }
    public void OnExitPressed()
    {
        pauseMenuActiveSelf = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
