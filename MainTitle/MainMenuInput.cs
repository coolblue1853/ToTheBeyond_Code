using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuInput : MonoBehaviour
{
    [SerializeField] private GameObject _pressAnyKeyText;
    [SerializeField] private ScreenFader _screenFader;
    [SerializeField] private string _sceneToLoad = "InGame_Build";

    private bool _hasPressed = false;

    private void OnEnable()
    {    
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        _hasPressed = false;
    
        if (_pressAnyKeyText != null)
            _pressAnyKeyText.SetActive(true);
    }
    
    void Update()
    {
        if (_hasPressed) return;

        if (Input.anyKeyDown)
        {
            _hasPressed = true;

            if (_pressAnyKeyText != null)
                _pressAnyKeyText.SetActive(false);

            StartCoroutine(FadeAndLoad());
        }
    }

    private IEnumerator FadeAndLoad()
    {
        yield return _screenFader.FadeOut();
        SceneManager.LoadScene(_sceneToLoad);
    }
}