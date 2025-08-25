using System.Collections;
using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    private bool _isPlayerNearby = false;
    [SerializeField] private bool _canEnter = false;
    [SerializeField] private bool _forceEnalbe = false;
    private bool _inputLocked = false;
    
    [SerializeField] private float _inputCooldown = 1f; 
    [SerializeField] private GameObject _promptUI;
    
    public void EnablePortal()
    {
        _canEnter = true;
        if (_promptUI != null)
            _promptUI.SetActive(false); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerNearby = true;
            if (_canEnter && _promptUI != null)
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
        if (!_isPlayerNearby || !_canEnter || _inputLocked) return;

        if (Input.GetKeyDown(KeyCode.V))
        {
            _inputLocked = true;
            if (_promptUI != null)
                            _promptUI.SetActive(false);
            
            MapManager.Instance.GoToNextMap();
            ResetPortal();
            StartCoroutine(UnlockInputAfterDelay());
        }
    }

    private IEnumerator UnlockInputAfterDelay()
    {
        yield return new WaitForSeconds(_inputCooldown);
        _inputLocked = false;
    }

    
    public void ResetPortal()
    {
        if (_forceEnalbe)
            return;

        _canEnter = false;
        _isPlayerNearby = false;
        if (_promptUI != null)
            _promptUI.SetActive(false);
    }
}
