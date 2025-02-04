using System;
using UnityEngine;

public class FlushButton : MonoBehaviour
{
    [SerializeField] WaterClosetCloseChecker waterClosetCloseChecker;
    AudioSource audioSource;

    bool isPressed = false;
    public Action<bool> OnPressed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("LeftHand") || other.gameObject.CompareTag("RightHand"))
        {
            Debug.Log("야야 left hand");

            if (isPressed) return;

            if (!waterClosetCloseChecker.IsClosed)
            {
                PlayerTestDebug.Instance.ShowAlertText("덮개를 덮고 눌러주세요");
                return;
            }

            audioSource.Play();
            isPressed = true;
            OnPressed?.Invoke(true);
        }
    }
}
