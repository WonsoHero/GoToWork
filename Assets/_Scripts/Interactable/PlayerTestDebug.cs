using TMPro;
using UnityEngine;

/// <summary>
///  디버그용 대충 화면에 텍스트 출력하기
/// </summary>
public class PlayerTestDebug : MonoBehaviour
{
    [SerializeField] TMP_Text InformationText;
    [SerializeField] TMP_Text AlertText;

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
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeInformationText(string text)
    {
        if (InformationText == null) return;

        InformationText.text = text;
    }

    /// <summary>
    ///  UI 상단 사라지는 텍스트
    /// </summary>
    /// <param name="text"></param>
    public void ShowAlertText(string text)
    {
        if (AlertText == null) return;

        AlertText.text = text;
        AlertText.CrossFadeAlpha(1.0f, 0.01f, true);
        AlertText.CrossFadeAlpha(0.0f, 2f, true);
    }
}
