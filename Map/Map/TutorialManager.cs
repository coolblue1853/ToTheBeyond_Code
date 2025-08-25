using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private bool _inTutorial = false;
    private int _tutorialIndex = -1;
    private bool _isTransitioning = false;
    
    public ScreenFader fader;
    public GameObject faderObject;
    MapManager mapManager;
    
    [SerializeField] private float _transitionDuration;
    [SerializeField] private float _transitionSpawnDuration;
    [SerializeField] private float _loadingDuration = 1.5f;
    [SerializeField] private List<MapComponent> _tutorialMaps = new();

    public static TutorialManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private void Start()
    {
        mapManager = MapManager.Instance;
    }

    private void OnEnable()
    {
        MapManager.OnCombatStarted += EndTutorial;
        MapManager.OnTownReset += ClearTutorialMaps;
    }

    private void OnDisable()
    {
        MapManager.OnCombatStarted -= EndTutorial;
        MapManager.OnTownReset -= ClearTutorialMaps;
    }

    public void EnterTutorial()
    {
        if (_isTransitioning) return;
        _inTutorial = true;
        StartCoroutine(StartTutorialRoutine());
    }

    private IEnumerator StartTutorialRoutine()
    {
        GameManager.Instance.playerController.isControllable = false;
        faderObject.SetActive(true);
        fader.SetAlpha(1f);
        yield return new WaitForSeconds(_loadingDuration/2);

        mapManager.DeactivateCurrentMap();
        
        _tutorialMaps.Clear();
        _tutorialIndex = 0;

        var tutorialPrefabs = Resources.LoadAll<GameObject>("Maps/Tutorial");
        foreach (var prefab in tutorialPrefabs)
        {
            var instance = Instantiate(prefab);
            instance.SetActive(false);
            _tutorialMaps.Add(instance.GetComponent<MapComponent>());
        }
        yield return new WaitForSeconds(_loadingDuration / 2);
        yield return ActivateTutorialMap(_tutorialIndex);
        yield return fader.FadeIn();
        faderObject.SetActive(false);
        _isTransitioning = false;
        GameManager.Instance.playerController.isControllable = true;
    }

    public void GoToNextTutorialMap()
    {
        if (_isTransitioning || !_inTutorial) return;
        StartCoroutine(TransitionToNextTutorialMap());
    }

    private IEnumerator TransitionToNextTutorialMap()
    {
        GameManager.Instance.playerController.isControllable = false;
        _isTransitioning = true;
        faderObject.SetActive(true);
        yield return fader.FadeOut(false);

        if (_tutorialIndex >= 0 && _tutorialIndex < _tutorialMaps.Count)
            _tutorialMaps[_tutorialIndex].Deactivate();

        _tutorialIndex++;

        if (_tutorialIndex < _tutorialMaps.Count)
        {
            yield return ActivateTutorialMap(_tutorialIndex);
            yield return fader.FadeIn();
        }
        else
        {
            _inTutorial = false;
            GameManager.Instance.HandleRespawn();
        }

        faderObject.SetActive(false);
        _isTransitioning = false;
        GameManager.Instance.playerController.isControllable = true;
    }
    
    private IEnumerator ActivateTutorialMap(int index)
    {
        var map = _tutorialMaps[index];
        return mapManager.ActivateMapCommon(map);
    }

    public void ClearTutorialMaps()
    {
        foreach (var map in _tutorialMaps)
            if (map != null) Destroy(map.gameObject);
        _tutorialMaps.Clear();
    }

    public void EndTutorial()
    {
        _inTutorial = false;
    }
}
