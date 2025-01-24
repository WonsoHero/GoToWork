using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 그냥 두기 뭐해서...
/// 메뉴화면 타이틀 이미지 커졌다 작아졌다 하는 애니메이션
/// </summary>
public class ImageScaleAnimation : MonoBehaviour
{
    private Image image;

    [SerializeField]
    Vector3 minScale = new Vector3(1f, 1f, 1f);
    
    [SerializeField]
    Vector3 maxScale = new Vector3(1.05f, 1.05f, 1.05f);

    public float scalingSpeed;
    public float scalingDuration;


    WaitForSeconds WaitForSeconds = new WaitForSeconds(0.01f);
    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private IEnumerator Start()
    {
        while(true)
        {
            yield return ScaleLerp(minScale, maxScale, scalingDuration);
            yield return ScaleLerp(maxScale, minScale, scalingDuration);
        }
    }

    private IEnumerator ScaleLerp(Vector3 startScale, Vector3 endScale, float time)
    {
        float t = 0.0f;
        float rate = (1f / time) * scalingSpeed;
        while(t < 1f) {
            t += Time.deltaTime * rate;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
    }
}
