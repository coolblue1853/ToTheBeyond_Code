using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class RewardCountChance
{
    public int count;
    public float probability;
}

public class RewardManager : MonoBehaviour
{
    // 던전 클리어시 보상을 반환하는 관리자 
    public static RewardManager Instance;

    [Header("보상 데이터")]
    [SerializeField] private List<RewardDataSO> _rewardPool;

    [Header("보상 스폰 위치 및 간격")]
    [SerializeField] private float _spacing = 0.5f;

    [Header("등장 갯수 확률 (합은 1.0이 되어야 함)")]
    [SerializeField]
    private List<RewardCountChance> _rewardCountChances = new List<RewardCountChance>
    {
        new RewardCountChance { count = 1, probability = 0.6f },
        new RewardCountChance { count = 2, probability = 0.3f },
        new RewardCountChance { count = 3, probability = 0.1f },
    };

    [Header("등급별 등장 확률 (합은 1.0이 아니어도 됨)")]
    private Dictionary<RarityType, float> _rarityChances = new Dictionary<RarityType, float>
    {
        { RarityType.Common, 0.5f },     // 50%
        { RarityType.Rare, 0.25f },      // 25%
        { RarityType.Unique, 0.18f },    // 18%
        { RarityType.Epic, 0.05f },      // 5%
        { RarityType.Legendary, 0.02f }  // 2%
    };

    [Header("강제 조건")]
    [SerializeField] private int _minRewardCount = 2;                 // 최소 보상 개수
    [SerializeField] private bool _forceAtLeastOneWeapon = true;      // 무기 최소 1개
    [SerializeField] private bool _forceAtLeastOneExperience = true;  // 경험치 최소 1개

    private int _seed = 1111;
    private System.Random _rand;

    public List<RewardDataSO> GetRewardPoolSafe() => _rewardPool ?? new List<RewardDataSO>();




    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(this.gameObject); return; }

        LoadRewardPool(); // 자동 등록 추가
    }

    private void LoadRewardPool()
    {
        _rewardPool = Resources.LoadAll<RewardDataSO>("Reward").ToList();
    }

    public void SetRewardSpawnTransform(Transform spawnTransform)
    {
        //_rewardSpawnTransform = spawnTransform;
    }

    // 보상 생성 함수 
    public void GenerateRewards(Transform spawnTransform)
    {
        _seed = System.DateTime.Now.Millisecond + UnityEngine.Random.Range(0, 10000);
        _rand = new System.Random(_seed);

        int rewardCount = Mathf.Max(_minRewardCount, GetRandomRewardCount());

        List<RewardDataSO> selectedRewards = new List<RewardDataSO>();
        List<RewardDataSO> availablePool = _rewardPool.ToList();

        // 1) 기본 뽑기
        for (int i = 0; i < rewardCount; i++)
        {
            RarityType selectedRarity = GetRandomRarity();

            List<RewardDataSO> candidates = availablePool
                .Where(r => r.rarityType == selectedRarity).ToList();

            if (candidates.Count == 0)
                candidates = availablePool.Where(r => r.rarityType == RarityType.Rare).ToList();
            if (candidates.Count == 0)
                candidates = availablePool.ToList(); // 그래도 없으면 전체에서

            if (candidates.Count == 0) break;

            RewardDataSO chosen = candidates[_rand.Next(candidates.Count)];
            selectedRewards.Add(chosen);
            availablePool.Remove(chosen);
        }

        // 2) 강제 조건: 무기/경험치 포함
        var forcedTypes = new HashSet<RewardType>();
        ForceIncludeTypeIfNeeded(RewardType.Weapon, _forceAtLeastOneWeapon, selectedRewards, availablePool, rewardCount, forcedTypes);
        ForceIncludeTypeIfNeeded(RewardType.Experience, _forceAtLeastOneExperience, selectedRewards, availablePool, rewardCount, forcedTypes);

        SpawnRewards(selectedRewards, spawnTransform);
    }

    // 특정 타입을 최소 1개 강제 포함
    private void ForceIncludeTypeIfNeeded(
        RewardType type,
        bool enabled,
        List<RewardDataSO> selected,
        List<RewardDataSO> available,
        int targetCount,
        HashSet<RewardType> forcedTypes)
    {
        if (!enabled) return;
        if (selected.Any(r => r.rewardType == type)) return;

        var candidates = available.Where(r => r.rewardType == type).ToList();
        if (candidates.Count == 0) return;

        var forced = candidates[_rand.Next(candidates.Count)];
        selected.Add(forced);
        available.Remove(forced);
        forcedTypes.Add(type);

        // 개수 초과면 제거 (강제 타입은 우선 보호)
        if (selected.Count > targetCount)
        {
            var toRemove = selected
                .Where(r => !forcedTypes.Contains(r.rewardType)) // 강제 포함한 타입 보호
                .OrderBy(r => GetRarityWeight(r.rarityType))     // 낮은 가중치(흔함) 우선 제거
                .FirstOrDefault();

            if (toRemove == null)
            {
                // 전부 강제 타입이면 어쩔 수 없이 하나 제거 (가장 낮은 가중치)
                toRemove = selected.OrderBy(r => GetRarityWeight(r.rarityType)).First();
            }

            selected.Remove(toRemove);
            // 제거한 것은 풀에 되돌릴지는 선택 사항. 여기서는 되돌리지 않음.
        }
    }

    private float GetRarityWeight(RarityType rarity)
    {
        return _rarityChances.TryGetValue(rarity, out var w) ? w : 0f;
    }

    // 등장할 보상의 갯수 
    private int GetRandomRewardCount()
    {
        float roll = (float)_rand.NextDouble();
        float total = 0f;

        foreach (var entry in _rewardCountChances.OrderBy(e => e.probability))
        {
            total += entry.probability;
            if (roll <= total)
                return entry.count;
        }
        return _rewardCountChances.Count > 0 ? _rewardCountChances.Max(e => e.count) : 1;
    }

    // 등장할 보상의 레어도 
    private RarityType GetRandomRarity()
    {
        float roll = (float)_rand.NextDouble();
        float total = 0f;

        foreach (var kvp in _rarityChances.OrderBy(kvp => kvp.Value))
        {
            total += kvp.Value;
            if (roll <= total)
                return kvp.Key;
        }
        // 누적 합보다 클 경우 가장 높은 가중치(흔한 등급)로 폴백
        return _rarityChances.OrderByDescending(kvp => kvp.Value).First().Key;
    }

    private void SpawnRewards(List<RewardDataSO> rewards, Transform spawnTransform)
    {
        if (spawnTransform == null) return;

        Transform parentMap = spawnTransform.parent; // 맵의 루트로 설정
        Vector3 startPos = spawnTransform.position - Vector3.right * _spacing * (rewards.Count - 1) / 2f;

        for (int i = 0; i < rewards.Count; i++)
        {
            var data = rewards[i];
            if (data == null || data.prefab == null) continue;

            Instantiate(
                data.prefab,
                startPos + Vector3.right * _spacing * i,
                Quaternion.identity,
                parentMap // 보상 오브젝트를 맵의 자식으로 둠
            );
        }
    }

    public List<RewardDataSO> GetRandomPassiveOffers(int count, bool unique = true)
    {
        var pool = GetRewardPoolSafe().Where(r => r.rewardType == RewardType.PassiveItem).ToList();
        if (pool.Count == 0) return new List<RewardDataSO>();

        var list = new List<RewardDataSO>();
        for (int i = 0; i < Mathf.Min(count, pool.Count); i++)
        {
            int idx = UnityEngine.Random.Range(0, pool.Count);
            list.Add(pool[idx]);
            if (unique) pool.RemoveAt(idx);
        }
        return list;
    }

    public int GetShopPrice(RarityType rarity)
    {
        // 네가 쓰던 가격 규칙 사용
        return rarity switch
        {
            RarityType.Common => 100,
            RarityType.Rare => 200,
            RarityType.Unique => 350,
            RarityType.Epic => 500,
            RarityType.Legendary => 800,
            _ => 100
        };
    }
}
