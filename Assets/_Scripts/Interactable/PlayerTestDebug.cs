using TMPro;
using UnityEngine;

/// <summary>
///  디버그용 대충 화면에 텍스트 출력하기
/// </summary>
public class PlayerTestDebug : MonoBehaviour
{
    [SerializeField] TMP_Text DebugText;

    private static PlayerTestDebug instance;
    public static PlayerTestDebug Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeDebugText(string text)
    {
        DebugText.text = text;
    }
}
