using UnityEngine;

public class PlayerUpgradeTrigger : MonoBehaviour
{
    public GameObject panel;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PermanentUpgradeManager.Instance.SetPlayerInRange(true);
            panel.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PermanentUpgradeManager.Instance.SetPlayerInRange(false);
            panel.SetActive(false);
        }
    }
}
