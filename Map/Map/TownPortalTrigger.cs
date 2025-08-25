using UnityEngine;

public class TownPortalTrigger : MonoBehaviour
{
    private bool _isPlayerNearby = false;
    private bool _inputLocked = false;
    private bool _hasEnteredCombat = false;
    
    [SerializeField] private float _inputCooldown = 5f;
    [SerializeField] private GameObject _promptUI;
    
    private void OnEnable()
    {
        _hasEnteredCombat = false;
        if (_promptUI != null)
            _promptUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _isPlayerNearby = true;
        
        if (!_hasEnteredCombat && _promptUI != null)
            _promptUI.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _isPlayerNearby = false;
        
        if (_promptUI != null)
            _promptUI.SetActive(false);
    }

    private void Update()
    {
        if (!_isPlayerNearby || _inputLocked || _hasEnteredCombat) return;

        if (Input.GetKeyDown(KeyCode.V))
        {
            _inputLocked = true;
            _hasEnteredCombat = true;
            
            if (_promptUI != null)
                _promptUI.SetActive(false);

            MapManager.Instance.EnterCombat(); // 씬 전환

        }
    }
}
