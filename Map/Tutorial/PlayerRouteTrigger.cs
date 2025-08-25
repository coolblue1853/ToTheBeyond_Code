using UnityEngine;

public class PlayerRouteTrigger : MonoBehaviour
{
    [SerializeField] private int _dialogueIndex; // 출력할 대사 인덱스
    [SerializeField] private PinchBirdSpeech _pinchBirdSpeech;

    //private bool _isTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if (_isTriggered) return;
        if (!other.CompareTag("Player")) return;

        //_isTriggered = true;

        // 대사 출력
        //PinchBirdSpeech.ShowDialogue(_dialogueIndex);
        _pinchBirdSpeech.ShowDialogue(_dialogueIndex);

    }
}