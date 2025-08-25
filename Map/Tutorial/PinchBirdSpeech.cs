using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PinchBirdSpeech : MonoBehaviour
{
    //public static PinchBirdSpeech Instance { get; private set; }
    
    [Header("Speech Bubble")]
    [SerializeField] private GameObject _speechBubble;
    
    [Header("PinchBird Target")]
    [SerializeField] private Transform _target; 
    [SerializeField] private Vector3 _offset = new Vector3(0, 2f, 0);  
    
    [Header("대사 리스트")]
    [TextArea]
    [SerializeField] private string[] _dialogues = new string[]
    {
        
    };
    
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private Camera _mainCamera;
    private TextMeshProUGUI _dialogueText;
    
    private void Awake()
    {
        //if (Instance == null) Instance = this;
        
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _mainCamera = Camera.main;
    }

    private void Start()
    {
          Canvas.ForceUpdateCanvases();
          LayoutRebuilder.ForceRebuildLayoutImmediate(_speechBubble.GetComponent<RectTransform>());
    }

    private void LateUpdate()
    {
        if (_target == null || _canvas == null || _rectTransform == null) return;

        if (_speechBubble.activeSelf)
        {
            Vector3 worldPos = _target.position + _offset;
            Vector3 screenPos = _mainCamera.WorldToScreenPoint(worldPos);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, screenPos, null, out Vector2 localPos);

            _rectTransform.localPosition = localPos;
        }
    }

    public void ShowDialogue(int index)
    {
       
        _dialogueText = FindObjectsOfType<TextMeshProUGUI>(false).FirstOrDefault(tmp => tmp.gameObject.name == "PinchBirdText");

        if (index < 0 || index >= _dialogues.Length) return;

        _speechBubble.SetActive(false); 
        _speechBubble.SetActive(true);  

        _dialogueText.text = _dialogues[index];

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_speechBubble.GetComponent<RectTransform>());
    }

    public void HideSpeech()
    {
        _speechBubble.SetActive(false);
    }
}