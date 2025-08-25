using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MainMenuBGM : MonoBehaviour
{
    [SerializeField] private AudioClip _bgmClip; 
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _bgmClip;
        _audioSource.loop = true;

        float volume = PlayerPrefs.GetFloat("MasterVolume", 1f); 
        _audioSource.volume = volume*0.2f;

        _audioSource.Play();
    }
}