using System.Collections.Generic;
using UnityEngine;

public class MapCycleManager : MonoBehaviour
{
    [Header("Theme / Counts")]
    public string currentTheme = "Forest";
    public int minNormalCount = 2;
    public int maxNormalCount = 3;

    [Header("Toggles")]
    public bool includeShopAfterMidBoss = true; // ✅ 미드보스 뒤에 상점 포함

    public GameObject LoadTownMap()
    {
        return Resources.Load<GameObject>("Maps/Town/TownMap");
    }

    public List<GameObject> BuildCombatCycle()
    {
        List<GameObject> result = new();

        // 1) 일반맵 타입1
        var normal1List = new List<GameObject>(Resources.LoadAll<GameObject>($"Maps/{currentTheme}/Normal1"));
        var firstNormals = PickRandom(normal1List, minNormalCount, maxNormalCount);
        result.AddRange(firstNormals);

        // 2) 미드보스
        var midBoss = Resources.Load<GameObject>($"Maps/{currentTheme}/MidBoss/{currentTheme}_MidBoss");
        if (midBoss != null) result.Add(midBoss);

        // ✅ 2.5) 상점 (Shop 폴더에서 1개 랜덤 선택)
        if (includeShopAfterMidBoss)
        {
            var shop = LoadRandomShopMap();
            if (shop != null) result.Add(shop);
        }

        // 3) 일반맵 타입2
        var normal2List = new List<GameObject>(Resources.LoadAll<GameObject>($"Maps/{currentTheme}/Normal2"));
        var secondNormals = PickRandom(normal2List, minNormalCount, maxNormalCount);
        result.AddRange(secondNormals);

        // 4) 최종보스
        var finalBoss = Resources.Load<GameObject>($"Maps/{currentTheme}/FinalBoss/{currentTheme}_FinalBoss");
        if (finalBoss != null) result.Add(finalBoss);

        return result;
    }

    // Shop 폴더에서 하나 랜덤 로드 (없으면 null)
    private GameObject LoadRandomShopMap()
    {
        // 우선 폴더 전체에서 로드
        var shops = new List<GameObject>(Resources.LoadAll<GameObject>($"Maps/{currentTheme}/Shop"));
        if (shops.Count > 0)
        {
            int idx = Random.Range(0, shops.Count);
            return shops[idx];
        }

        // 폴더가 비어있으면 네이밍 규칙 시도: {Theme}_Shop
        var fallback = Resources.Load<GameObject>($"Maps/{currentTheme}/Shop/{currentTheme}_Shop");
        return fallback; // null이면 호출부에서 자동 스킵
    }

    private List<GameObject> PickRandom(List<GameObject> source, int min, int max)
    {
        int count = Mathf.Min(Random.Range(min, max + 1), source.Count);
        List<GameObject> result = new();
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, source.Count);
            result.Add(source[index]);
            source.RemoveAt(index);
        }
        return result;
    }
}
