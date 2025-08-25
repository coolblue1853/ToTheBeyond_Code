using UnityEngine;

public class ReturnToTownPortal : MonoBehaviour
{
    private bool _isPlayerNearby = false;
    private bool _inputLocked = false;
    
    [SerializeField] private float _inputCooldown = 1f;
    [SerializeField] private GameObject _promptUI;
    
    private void OnEnable()
    {
        _inputLocked = false;
        if (_promptUI != null)
            _promptUI.SetActive(false); // 시작 시 꺼두기
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerNearby = true;
            if (_promptUI != null)
                _promptUI.SetActive(true);
        }
     
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerNearby = false;
            if (_promptUI != null)
                _promptUI.SetActive(false);
        }
     
    }

    private void Update()
    {
        if (!_isPlayerNearby || _inputLocked) return;

        if (Input.GetKeyDown(KeyCode.V))
        {
            _inputLocked = true;
            
            if (_promptUI != null)
                _promptUI.SetActive(false);
            
            GameManager.Instance.HandleRespawn();
        }
    }


}