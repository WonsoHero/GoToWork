using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    AudioSource source;
    [SerializeField] AudioMixer bgmMixer;
    [SerializeField] AudioMixer sfxMixer;
    [SerializeField] AudioClip mainMenuBGM;

    static SoundManager instance;
    static public SoundManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        source = GetComponent<AudioSource>();

        PlayBGM(mainMenuBGM);
    }

    public void PlayBGM(AudioClip bgm)
    {
        bgmMixer.GetFloat(SoundSetting.mixerMusicVolumeKey, out float volume);
        float realVolume = Mathf.Pow(10f, volume / 80f);
        source.PlayOneShot(bgm, realVolume);
    }

    public void PlaySFX(AudioClip sfx)
    {
        sfxMixer.GetFloat(SoundSetting.mixerMusicVolumeKey, out float volume);
        float realVolume = Mathf.Pow(10f, volume / 80f);
        source.PlayOneShot(sfx, realVolume);
    }
}
