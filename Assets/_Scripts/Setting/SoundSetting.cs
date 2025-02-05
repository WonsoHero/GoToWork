using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;

    [SerializeField] AudioMixer bgmMixer;
    [SerializeField] AudioMixer sfxMixer;

    public readonly static string SavedBGMVolumeKey = "SavedBGMVolume";
    public readonly static string SavedSFXVolumeKey = "SavedSFXVolume";

    public readonly static string mixerMusicVolumeKey = "MusicVolume";

    private void Start()
    {
        SetBGMVolume(PlayerPrefs.GetFloat(SavedBGMVolumeKey, 50));
        SetSFXVolume(PlayerPrefs.GetFloat(SavedSFXVolumeKey, 50));
    }

    public void SetBGMVolume(float value)
    {
        if (value < 1)
        {
            value = 0.001f;
        }

        RefreshBGMSlider(value);
        PlayerPrefs.SetFloat(SavedBGMVolumeKey, value);
        bgmMixer.SetFloat(mixerMusicVolumeKey, Mathf.Log10(value / 100) * 80f);
    }

    public void SetBGMVolumeFromSlider()
    {
        SetBGMVolume(bgmSlider.value);
    }

    public void RefreshBGMSlider(float value)
    {
        bgmSlider.value = value;
    }

    public void SetSFXVolume(float value)
    {
        if (value < 1)
        {
            value = 0.001f;
        }

        RefreshSFXSlider(value);
        PlayerPrefs.SetFloat(SavedSFXVolumeKey, value);
        sfxMixer.SetFloat(mixerMusicVolumeKey, Mathf.Log10(value / 100) * 80f);
    }

    public void SetSFXVolumeFromSlider()
    {
        SetSFXVolume(sfxSlider.value);
    }

    public void RefreshSFXSlider(float value)
    {
        sfxSlider.value = value;
    }
}
