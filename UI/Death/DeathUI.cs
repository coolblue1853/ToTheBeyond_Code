using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DeathUI : MonoBehaviour
{
    public GameObject panel;
    public GameObject background;
    public RawImage deathMomentImage;
    public GameObject damagePanel;

    private bool _isVisible = false;

    public void Show()
    {
        StartCoroutine(CaptureThenShow());
    }

    private IEnumerator CaptureThenShow()
    {
        panel.SetActive(false);
        background.SetActive(false);
        _isVisible = false;
        damagePanel.SetActive(false);
        
        yield return new WaitForEndOfFrame();

        Texture2D screenshot = ScreenshotCapture.Instance.Capture();

        damagePanel.SetActive(true);
        background.SetActive(true);
        panel.SetActive(true);
        _isVisible = true;
        
        Time.timeScale = 0f;

        if (deathMomentImage != null)
        {
            deathMomentImage.texture = screenshot;
            deathMomentImage.color = new Color(1f, 1f, 1f, 1f); // 연하게
        }
    }

    public void Hide()
    {
        background.SetActive(false);
        panel.SetActive(false);
        _isVisible = false;
    }

    private void Update()
    {
        if (_isVisible && Input.GetKeyDown(KeyCode.V))
        {
            Time.timeScale = 1f;
            GameManager.Instance.HandleRespawn();
        }
    }
}