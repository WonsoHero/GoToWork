using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
///  버튼에 마우스 오버했을 때 효과 적용
/// </summary>
public class ButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public float ScaleOnMouseOver = 0.9f;

    private Button button;
    private TMP_Text text;

    private void Awake()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<TMP_Text>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = Vector3.one * ScaleOnMouseOver;
        text.alpha = 0.7f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
        text.alpha = 1f;
    }
}
