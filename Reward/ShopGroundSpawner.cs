using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShopGroundSpawner : MonoBehaviour
{
    [SerializeField] private GameObject shopOfferPrefab;   // ShopGroundOffer가 붙은 프리팹
    [SerializeField] private Transform[] spawnPoints;      // 여기서 들고있음!
    [SerializeField] private int offerCount = 2;
    [SerializeField] private bool uniqueInPage = true;

    private readonly List<ShopGroundOffer> spawned = new();

    [Header("Force Slot 0")]
    [SerializeField] private RewardDataSO forcedHealReward; // ← 인스펙터에서 지정

    private void Start()
    {
        SpawnOffers();
    }

    public void SpawnOffers()
    {
        ClearOffers();

        var pool = RewardManager.Instance
            .GetRewardPoolSafe()
            .Where(r => r != null && r.rewardType == RewardType.PassiveItem)
            .ToList();

        if (pool.Count == 0 || spawnPoints == null || spawnPoints.Length == 0) return;

        int spawnedCount = 0;

        // 0번 슬롯: 강제 힐 아이템
        if (forcedHealReward != null)
        {
            SpawnOneAtIndex(0, forcedHealReward);
            pool.Remove(forcedHealReward);
            spawnedCount++;
        }

        // 나머지 슬롯: 랜덤
        for (int i = spawnedCount; i < offerCount && i < spawnPoints.Length; i++)
        {
            if (pool.Count == 0) break;
            int idx = Random.Range(0, pool.Count);
            var pick = pool[idx];
            if (uniqueInPage) pool.RemoveAt(idx);

            SpawnOneAtIndex(i, pick);
        }
    }


    public void Reroll()
    {
        ClearOffers();
        SpawnOffers();
    }

    public void ClearOffers()
    {
        foreach (var s in spawned)
            if (s) Destroy(s.gameObject);
        spawned.Clear();
    }

    private void SpawnOneAtIndex(int slotIndex, RewardDataSO data)
    {
        if (slotIndex < 0 || slotIndex >= spawnPoints.Length) return;
        if (data == null) return;

        var spawnPoint = spawnPoints[slotIndex];
        var go = Instantiate(shopOfferPrefab, spawnPoint.position, Quaternion.identity, spawnPoint.parent);

        var offer = go.GetComponent<ShopGroundOffer>();
        if (offer != null)
        {
            // 가격은 RewardManager에서 등급별로 가져온다고 가정
            int price = RewardManager.Instance.GetShopPrice(data.rarityType);
            offer.Setup(data, price);
        }

        spawned.Add(offer);
    }

}
