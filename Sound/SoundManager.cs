using DarkTonic.MasterAudio;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    
    public float MasterVolume { get; private set; }
    public float Sfx { get; private set; }
    public float Bgm { get; private set; }
    
    private const string _SFX_BUS_NAME = "SFX";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetInitialSetting()
    {
        // 초기값 로드
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.7f);
        Sfx = PlayerPrefs.GetFloat("SFXVolume", 0.7f);
        Bgm = PlayerPrefs.GetFloat("BGMVolume", 0.7f);
    }


    public void SetMasterVolume(float value)
    {
        MasterVolume = value;
        MasterAudio.SetBusVolumeByName(_SFX_BUS_NAME, value*MasterVolume);
        MasterAudio.PlaylistMasterVolume = value*MasterVolume;
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        MasterAudio.SetBusVolumeByName(_SFX_BUS_NAME, value*MasterVolume);
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    public void SetBGMVolume(float value)
    {
        MasterAudio.PlaylistMasterVolume = value*MasterVolume;
        PlayerPrefs.SetFloat("BGMVolume", value);
        PlayerPrefs.Save();
    }
}
