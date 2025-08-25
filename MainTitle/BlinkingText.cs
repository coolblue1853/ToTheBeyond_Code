using System.Collections;
using UnityEngine;
using TMPro;

public class BlinkingText : MonoBehaviour
{
    private TextMeshProUGUI _text;

    void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        while (true)
        {
            _text.enabled = !_text.enabled;
            yield return new WaitForSeconds(0.5f);
        }
    }
}