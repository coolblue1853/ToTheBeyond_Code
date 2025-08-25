using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class PermanentUpgradeManager : MonoBehaviour
{
    public static PermanentUpgradeManager Instance;

    [System.Serializable]
    public class UpgradeData
    {
        public StatType statType;
        public int level;
        public float valuePerLevel;
        public int maxLevel;
    }

    public List<UpgradeData> upgrades;
    public int currency;

    [Header("UI")]
    [SerializeField] private GameObject upgradeUI;
    [SerializeField] private UpgradeUIManager upgradeUIManager;
    [SerializeField] private TextMeshProUGUI currencyText;

    private bool playerInRange = false;

    public bool IsUpgradeUIOpen = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadData();
            RefreshCurrencyText();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        Invoke("ApplyUpgradeToPlayer", 0.5f);   
    }
    private void Initialize()
    {
        ApplyUpgradeToPlayer();
    }



    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.V))
        {
            if ((SettingsManager.Instance != null && SettingsManager.Instance.IsSettingsOpen) || InventoryManager.Instance.IsInventoryOpen)
                return;

            OpenUI();
        }

        if (upgradeUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseUI();
        }
    }

    public void OpenUI()
    {
        upgradeUI.SetActive(true);
        Time.timeScale = 0f;
        IsUpgradeUIOpen = true;
        SettingsManager.Instance.openUICount += 1;
        upgradeUIManager.RefreshAll();
    }

    public void CloseUI()
    {
        upgradeUI.SetActive(false);
        Time.timeScale = 1f;
        Invoke("OpenUIDown", 0.1f);
    }

    void OpenUIDown()
    {
        IsUpgradeUIOpen = false;
    }

    public void SetPlayerInRange(bool inRange)
    {
        playerInRange = inRange;
    }

    public bool TryUpgrade(StatType statType)
    {
        var upgrade = upgrades.Find(u => u.statType == statType);
        if (upgrade == null || upgrade.level >= upgrade.maxLevel)
            return false;

        int cost = GetCostForLevel(upgrade.level);
        if (currency < cost) return false;

        currency -= cost;
        upgrade.level++;
        SaveData();
        RefreshCurrencyText();

        ApplyUpgradeToPlayer();

        return true;
    }

    public float GetUpgradeBonus(StatType statType)
    {
        var upgrade = upgrades.Find(u => u.statType == statType);
        return upgrade != null ? upgrade.level * upgrade.valuePerLevel : 0f;
    }

    public int GetCostForLevel(int level) => 100 + (level * 50);

    public void AddCurrency(int amount)
    {
        currency += amount;
        SaveData();
        RefreshCurrencyText();
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
        foreach (var u in upgrades) u.level = 0;
        currency = 0;
        RefreshCurrencyText();
    }

    private void SaveData()
    {
        foreach (var u in upgrades)
            PlayerPrefs.SetInt($"Upgrade_{u.statType}", u.level);

        PlayerPrefs.SetInt("PermanentCurrency", currency);
        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        foreach (var u in upgrades)
            u.level = PlayerPrefs.GetInt($"Upgrade_{u.statType}", 0);

        currency = PlayerPrefs.GetInt("PermanentCurrency", 0);
    }

    public void ResetAllUpgrades()
    {
        foreach (var upgrade in upgrades)
        {
            upgrade.level = 0;
            PlayerPrefs.SetInt($"Upgrade_{upgrade.statType}", 0);
        }

        currency = 0;
        PlayerPrefs.SetInt("PermanentCurrency", 0);
        PlayerPrefs.Save();

        RefreshCurrencyText();
        ApplyUpgradeToPlayer();
    }

    public void ApplyUpgradeToPlayer()
    {
        if (GameManager.Instance?.playerController != null)
        {
            var health = GameManager.Instance.playerController.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.ApplyPermanentUpgradeBonuses();
                HealthUIController.Instance?.UpdateUI();
            }
        }
    }

    private void RefreshCurrencyText()
    {
        if (currencyText != null)
        {
            currencyText.text = $"{currency}";
        }
    }
}
