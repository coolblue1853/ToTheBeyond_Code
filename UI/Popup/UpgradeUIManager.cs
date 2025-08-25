using TMPro;
using UnityEngine;

public class UpgradeUIManager : MonoBehaviour
{
    public static UpgradeUIManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private UpgradeSlotUI[] upgradeSlots;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        RefreshAll();
    }

    public void RefreshAll()
    {
        RefreshCurrency();
        foreach (var slot in upgradeSlots)
        {
            slot.Refresh();
        }
    }

    public void RefreshCurrency()
    {
        currencyText.text = $"{PermanentUpgradeManager.Instance.currency}";
    }
}
