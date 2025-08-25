using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _settingsUI;

    [Header("AudioUI Sliders")]
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _bgmSlider;

    [Header("Data Reset Options")]
    [Tooltip("세이브/설정 파일까지 모두 삭제 (Application.persistentDataPath 전체 정리)")]
    [SerializeField] private bool _alsoClearPersistentData = true;

    public int openUICount = 0;

    private SoundManager _soundManager;

    public static SettingsManager Instance { get; private set; }

    public bool IsSettingsOpen => _settingsUI != null && _settingsUI.activeSelf;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _soundManager = SoundManager.Instance;
        _soundManager.SetInitialSetting();
        SetSoundSetting();
    }

    private void OnDestroy()
    {
        if (_soundManager == null) return;
        if (_masterSlider != null) _masterSlider.onValueChanged.RemoveListener(_soundManager.SetMasterVolume);
        if (_sfxSlider != null) _sfxSlider.onValueChanged.RemoveListener(_soundManager.SetSFXVolume);
        if (_bgmSlider != null) _bgmSlider.onValueChanged.RemoveListener(_soundManager.SetBGMVolume);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 다른 UI 열려있으면 무시
            if (InventoryManager.Instance != null && InventoryManager.Instance.IsInventoryOpen) return;
            if (PermanentUpgradeManager.Instance != null && PermanentUpgradeManager.Instance.IsUpgradeUIOpen) return;

            bool willOpen = !_settingsUI.activeSelf;
            _settingsUI.SetActive(willOpen);
            Time.timeScale = willOpen ? 0f : 1f;
        }
    }

    public void QuitBtn()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void MainBtn()
    {
        Time.timeScale = 1f; // 정지 상태로 씬 이동 방지
        SceneManager.LoadScene("MainTitle");
    }

    private void SetSoundSetting()
    {
        _soundManager = SoundManager.Instance;

        if (_masterSlider != null) _masterSlider.value = _soundManager.MasterVolume;
        if (_sfxSlider != null) _sfxSlider.value = _soundManager.Sfx;
        if (_bgmSlider != null) _bgmSlider.value = _soundManager.Bgm;

        // 리스너 중복 방지 후 연결
        if (_masterSlider != null)
        {
            _masterSlider.onValueChanged.RemoveListener(_soundManager.SetMasterVolume);
            _masterSlider.onValueChanged.AddListener(_soundManager.SetMasterVolume);
        }
        if (_sfxSlider != null)
        {
            _sfxSlider.onValueChanged.RemoveListener(_soundManager.SetSFXVolume);
            _sfxSlider.onValueChanged.AddListener(_soundManager.SetSFXVolume);
        }
        if (_bgmSlider != null)
        {
            _bgmSlider.onValueChanged.RemoveListener(_soundManager.SetBGMVolume);
            _bgmSlider.onValueChanged.AddListener(_soundManager.SetBGMVolume);
        }

        // 최초 값 반영
        _soundManager.SetMasterVolume(_masterSlider != null ? _masterSlider.value : _soundManager.MasterVolume);
        _soundManager.SetSFXVolume(_sfxSlider != null ? _sfxSlider.value : _soundManager.Sfx);
        _soundManager.SetBGMVolume(_bgmSlider != null ? _bgmSlider.value : _soundManager.Bgm);
    }

    // -------------------------------
    // 데이터 초기화 버튼 (3가지 버전)
    // -------------------------------

    // 1) 모든 로컬 데이터 삭제만 (화면 유지)
    public void Btn_ClearAllLocalData_InPlace()
    {
        ClearPlayerPrefsAndPersistentData();
    }

    // 2) 삭제 후 메인 타이틀로 재시작
    public void Btn_ClearAllLocalData_AndGoTitle()
    {
        ClearPlayerPrefsAndPersistentData();

        // 혹시 설정창이 떠 있었다면 닫고, 타임스케일 복구
        if (_settingsUI != null) _settingsUI.SetActive(false);
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainTitle");
    }

    // 실제 삭제 로직
    private void ClearPlayerPrefsAndPersistentData()
    {
        // PlayerPrefs 전체 삭제
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // persistentDataPath 삭제 (옵션)
        if (_alsoClearPersistentData)
            SafeClearPersistentDataPath();
    }

    private void SafeClearPersistentDataPath()
    {
        string root = Application.persistentDataPath;
        if (!Directory.Exists(root)) return;

        try
        {
            foreach (var file in Directory.GetFiles(root))
            {
                try { File.Delete(file); } catch { /* ignore */ }
            }
            foreach (var dir in Directory.GetDirectories(root))
            {
                try { Directory.Delete(dir, true); } catch { /* ignore */ }
            }
        }
        catch { }
    }
}
