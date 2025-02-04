using UnityEngine;

/// <summary>
///  변기 열고 닫히는 것
/// </summary>
public class SinkTapController : MonoBehaviour
{
    [SerializeField] GameObject waterEffect;
    AudioSource audioSource;

    // 수도꼭지 열린 상태
    private bool isOpened = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("LeftHand") || other.gameObject.CompareTag("RightHand"))
        {
            if (isOpened)
            {
                //Debug.Log("isOpened FALSE");
                isOpened = false;
                audioSource.Stop();
                waterEffect.SetActive(false);
                transform.Rotate(new Vector3(-15, 0, 0));
            }
            else
            {
                //Debug.Log("isOpened TRUE");
                isOpened = true;
                audioSource.Play();
                waterEffect.SetActive(true);
                transform.Rotate(new Vector3(15, 0, 0));
            }
        }
    }
}
