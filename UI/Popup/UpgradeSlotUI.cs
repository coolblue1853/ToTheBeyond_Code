using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSlotUI : MonoBehaviour
{
    public StatType statType;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI statNameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI bonusText;
    [SerializeField] private Button upgradeButton;

    private PermanentUpgradeManager.UpgradeData data;

    private void Start()
    {
        upgradeButton.onClick.AddListener(OnClickUpgrade);
        Refresh();
    }

    public void Refresh()
    {
        data = PermanentUpgradeManager.Instance.upgrades.Find(u => u.statType == statType);

        if (data == null)
        {
            gameObject.SetActive(false);
            return;
        }

        switch (statType.ToString())
        {
            case "MaxHealth":
                statNameText.text = "최대 체력";
                break;
            case "DashCooldown":
                statNameText.text = "대쉬 쿨타임";
                break;
            case "PhysicalDamage":
                statNameText.text = "물리 데미지";
                break;
            case "SkillDamage":
                statNameText.text = "스킬 데미지";
                break;
            case "CritChance":
                statNameText.text = "치명타 확률";
                break;
            case "CritDamageMultiplier":
                statNameText.text = "크리티컬 데미지";
                break;
        }

        levelText.text = $"Lv {data.level} / {data.maxLevel}";

        int bonusInt = 0;

        if(data.level * data.valuePerLevel >= 1)
        {
            bonusInt = (int)(data.level * data.valuePerLevel);
            bonusText.text = $"+ {(bonusInt)}";
        }
        else if (data.level * data.valuePerLevel < 0)
        {
            bonusText.text = $"- {Mathf.Abs(data.level * data.valuePerLevel)}";
        }
        else
        {
            bonusInt = (int)Mathf.Round(data.level * data.valuePerLevel * 100);
            bonusText.text = $"+ {(bonusInt)}";
        }



        int cost = PermanentUpgradeManager.Instance.GetCostForLevel(data.level);
        costText.text = cost.ToString();

        bool canUpgrade = data.level < data.maxLevel &&
                          PermanentUpgradeManager.Instance.currency >= cost;

        upgradeButton.interactable = canUpgrade;
    }

    private void OnClickUpgrade()
    {
        if (PermanentUpgradeManager.Instance.TryUpgrade(statType))
        {
            Refresh();
            UpgradeUIManager.Instance.RefreshCurrency();
        }
    }
}

