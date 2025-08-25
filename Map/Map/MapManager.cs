using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }
    public MapCycleManager cycleManager;
    public ScreenFader fader;
    public GameObject faderObject;
    public CinemachineConfiner2D cameraConfiner;
    public CinemachineConfiner2D minimapcamConfiner;

    private RewardManager _rewardManager;
    public List<MapComponent> _mapInstances = new();

    private GameObject _playerInstance;
    private int _currentIndex = -1;
    private bool _isTransitioning = false;
    private bool _isInitialized = false;

    [SerializeField] private float _transitionDuration;
    [SerializeField] private float _transitionSpawnDuration;
    [SerializeField] private float _loadingDuration = 1.5f;

    [SerializeField] private Transform _backgroundFollowParent;
    private GameObject _currentBackground;

    public static event Action OnCombatStarted;
    public static event Action OnTownReset;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
       // DontDestroyOnLoad(gameObject);
        _rewardManager = GetComponent<RewardManager>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "InGame_Build") // 인게임 씬 이름으로 수정하세요
        {
         
        }
    }

    private void OnEnable()
    {
        InitializeMapState();
    }

    public void InitializeMapState()
    {
        _playerInstance = GameObject.FindWithTag("Player");
        if (_playerInstance == null) return;
        
        faderObject.SetActive(true);
        
        // 테스트용 리셋
        // TutorialSaveUtil.ResetTutorialProgress();

        // 튜토리얼 먼저 실행 여부 확인
        if (!TutorialSaveUtil.IsTutorialCompleted())
        {
            TutorialManager.Instance.EnterTutorial();
            return;
        }
        
        // 튜토리얼 완료자 → 마을 진입
        var townPrefab = cycleManager.LoadTownMap();
        var townInstance = Instantiate(townPrefab);
        townInstance.SetActive(false);
        _mapInstances.Add(townInstance.GetComponent<MapComponent>());

        _currentIndex = 0;
        StartCoroutine(StartInitialMap());
    }

    private IEnumerator StartInitialMap()
    {
        yield return ActivateMapRoutine(_currentIndex);
        yield return new WaitForSeconds(_loadingDuration);
        yield return StartCoroutine(fader.FadeIn());
        faderObject.SetActive(false);
    }

    public void GoToNextMap()
    {
        if (_isTransitioning) return;
        StartCoroutine(TransitionToNextMap());
    }

    private IEnumerator TransitionToNextMap()
    {
        GameManager.Instance.playerController.isControllable = false;
        _isTransitioning = true;
        faderObject.SetActive(true);
        yield return StartCoroutine(fader.FadeOut(false));

        if (_currentIndex >= 0 && _currentIndex < _mapInstances.Count)
            _mapInstances[_currentIndex].Deactivate();

        _currentIndex++;

        if (_currentIndex < _mapInstances.Count)
        {
            var map = _mapInstances[_currentIndex];
            map.gameObject.SetActive(true);
            
            yield return ActivateMapRoutine(_currentIndex);

            if (_playerInstance != null && map.playerSpawnPoint != null)
                _playerInstance.transform.position = map.playerSpawnPoint.position;

            if (map.cameraBounds != null)
            {
                cameraConfiner.BoundingShape2D = map.cameraBounds;
                minimapcamConfiner.BoundingShape2D = map.cameraBounds;

                cameraConfiner.InvalidateBoundingShapeCache();
                minimapcamConfiner.InvalidateBoundingShapeCache();
            }

            ObjectPooler.ReturnAllActiveObjects();
            yield return new WaitForSeconds(_transitionDuration);

            yield return StartCoroutine(fader.FadeIn());
            yield return new WaitForSeconds(_transitionSpawnDuration);
            var controller = _playerInstance.GetComponent<PlayerController>();
            if (controller != null)
                controller.currentMapRoot = map.mapRoot;
        }
        else
        {
            GameManager.Instance.ShowClearUI();
        }

        faderObject.SetActive(false);
        GameManager.Instance.playerController.isControllable = true;
        _isTransitioning = false;
    }

    public IEnumerator ResetToTown()
    {
        foreach (var map in _mapInstances)
            Destroy(map.gameObject);
        _mapInstances.Clear();
        
        // TutorialManager.Instance?.ClearTutorialMaps();
        OnTownReset?.Invoke();
        
        // 튜토리얼 완료 저장
        TutorialSaveUtil.MarkTutorialComplete();
        
        var townPrefab = cycleManager.LoadTownMap();
        var townInstance = Instantiate(townPrefab);
        townInstance.SetActive(false);
        _mapInstances.Add(townInstance.GetComponent<MapComponent>());

        _currentIndex = 0;
        yield return ActivateMapRoutine(_currentIndex);
    }

    public void EnterCombat()
    {
        if (_isTransitioning) return;
        // TutorialManager.Instance?.EndTutorial();
        
        OnCombatStarted?.Invoke();
        StartCoroutine(StartCombatRoutine());
    }

    private IEnumerator StartCombatRoutine()
    {
        GameManager.Instance.playerController.isControllable = false;
        faderObject.SetActive(true);
        yield return fader.FadeOut();
        yield return new WaitForSeconds(_loadingDuration /2);

        if (_currentIndex >= 0 && _currentIndex < _mapInstances.Count)
            _mapInstances[_currentIndex].Deactivate();

        _mapInstances.Clear();
        _currentIndex = 0;

        var combatPrefabs = cycleManager.BuildCombatCycle();
        foreach (var prefab in combatPrefabs)
        {
            var instance = Instantiate(prefab);
            instance.SetActive(false);
            _mapInstances.Add(instance.GetComponent<MapComponent>());
        }
        yield return new WaitForSeconds(_loadingDuration / 2);
        yield return ActivateMapRoutine(_currentIndex);
        yield return fader.FadeIn();
        faderObject.SetActive(false);
        _isTransitioning = false;
        GameManager.Instance.playerController.isControllable = true;
    }

    private IEnumerator ActivateMapRoutine(int index)
    {
        var map = _mapInstances[index];
        return ActivateMapCommon(map);
    }
    

    public IEnumerator ActivateMapCommon(MapComponent map)
    {
        if(GameManager.Instance != null && GameManager.Instance.playerController != null)
            GameManager.Instance.playerController.isControllable = false;
        map.gameObject.SetActive(true);

        if (_playerInstance != null && map.playerSpawnPoint != null)
            _playerInstance.transform.position = map.playerSpawnPoint.position;

        if (_currentBackground != null)
            Destroy(_currentBackground);

        yield return new WaitForSeconds(0.2f);

        if (map.backgroundPrefab != null && _backgroundFollowParent != null)
        {
            _currentBackground = Instantiate(map.backgroundPrefab, _backgroundFollowParent);
            _currentBackground.transform.localPosition = new Vector3(0, 0, 30);
            _currentBackground.transform.localRotation = Quaternion.identity;
            _currentBackground.transform.localScale = Vector3.one;
        }

        var controller = _playerInstance.GetComponent<PlayerController>();
        if (controller != null)
            controller.currentMapRoot = map.mapRoot;

        if (map.cameraBounds != null)
        {
            cameraConfiner.BoundingShape2D = map.cameraBounds;
            minimapcamConfiner.BoundingShape2D = map.cameraBounds; 

            cameraConfiner.InvalidateBoundingShapeCache();
            minimapcamConfiner.InvalidateBoundingShapeCache();
        }

        var vcam = FindObjectOfType<CinemachineVirtualCamera>();
        if (vcam != null)
            vcam.Follow = _playerInstance.transform;

        MapMusicManager.PlayMusicForMap(map);

        yield return new WaitForSeconds(0.5f);
        map.Activate();
        GameManager.Instance.playerController.isControllable = true;
    }

    public void DeactivateCurrentMap()
    {
        if (_currentIndex >= 0 && _currentIndex < _mapInstances.Count)
            _mapInstances[_currentIndex].Deactivate();
    }
}
