using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Rendering.DebugUI;

public class SoundOnDestruct : MonoBehaviour
{
    [SerializeField] AudioClip destructSound;
    [SerializeField] AudioMixer mixer;

    private void OnDisable()
    {
        if (!gameObject.scene.isLoaded) return;
        if (destructSound != null)
        {
            mixer.GetFloat(SoundSetting.mixerMusicVolumeKey, out float volume);

            float realVolume = Mathf.Pow(10f, volume / 80f);
            Debug.Log("Volume: " + volume + ", RealVolume" + realVolume);
            AudioSource.PlayClipAtPoint(destructSound, transform.position, realVolume);
        }
    }
}
