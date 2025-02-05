using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ClearArea : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Color textTarget;
    [SerializeField] float fadeTime = 2f;
    [SerializeField] GameObject volumeObject;
    
    WaitForFixedUpdate waitForFixedUpdate;
    Color textOrigin;
    float time;

    private void Awake()
    {
        textOrigin = text.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            time = Time.time;
            volumeObject.SetActive(true);
            StartCoroutine(ColorFade());
        }
    }

    IEnumerator ColorFade()
    {
        while (text.color.a < 1)
        {
            yield return waitForFixedUpdate;

            text.color = Color.Lerp(textOrigin, textTarget, (Time.time - time) / fadeTime);
        }
    }
}
