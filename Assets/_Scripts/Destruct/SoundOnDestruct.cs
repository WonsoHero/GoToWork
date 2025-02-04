using Unity.VisualScripting;
using UnityEngine;

public class SoundOnDestruct : MonoBehaviour
{
    [SerializeField] AudioClip destructSound;
    [SerializeField] float volume = 0.5f;

    private void OnDisable()
    {
        if (destructSound != null)
        {
            AudioSource.PlayClipAtPoint(destructSound, transform.position, volume);
        }
    }
}
