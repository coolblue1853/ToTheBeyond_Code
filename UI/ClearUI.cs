// ClearUI.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClearUI : MonoBehaviour
{
    public GameObject panel;
    public GameObject background;
    public RawImage deathMomentImage;

    private bool _isVisible = false;

    public void Show()
    {
        GameManager.Instance.playerController.gravityCtrl.Suppress();
        StartCoroutine(CaptureThenShow());
    }

    private IEnumerator CaptureThenShow()
    {
        panel.SetActive(false);
        background.SetActive(false);
        _isVisible = false;

        yield return new WaitForEndOfFrame();

        Texture2D screenshot = ScreenshotCapture.Instance.Capture();

        background.SetActive(true);
        panel.SetActive(true);
        _isVisible = true;

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
            GameManager.Instance.HandleRespawn();
        }
    }
}