using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ShopGroundOffer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI priceText;      // 월드 스페이스
    [SerializeField] private GameObject prompt;          // "F 구매하기" 표시 오브젝트
    [SerializeField] private SpriteRenderer iconRenderer;// 아이템 아이콘(선택)

    [Header("연출")]
    [SerializeField] private Color canBuyColor = new Color(1f, 0.9f, 0.4f);
    [SerializeField] private Color cantBuyColor = new Color(0.5f, 0.5f, 0.5f);

    private RewardDataSO data;
    private int price;
    private bool playerIn;
    private PlayerCoinHandler coin;
    private PlayerController player;

    public void Setup(RewardDataSO data, int price)
    {
        this.data = data;
        this.price = price;

        // 아이콘 규약: prefab에 SpriteRenderer가 있으면 그 스프라이트 사용
        if (iconRenderer != null && data.prefab != null)
        {
            var sr = data.prefab.GetComponentInChildren<SpriteRenderer>();
            if (sr != null) iconRenderer.sprite = sr.sprite;
        }
        RefreshVisual();
    }

    void Awake()
    {
        // 트리거 세팅
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();
        coin = player != null ? player.GetComponent<PlayerCoinHandler>() : null;

        if (coin != null) coin.OnCoinChanged += OnCoinChanged;
        RefreshVisual();
    }

    void OnDestroy()
    {
        if (coin != null) coin.OnCoinChanged -= OnCoinChanged;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIn = true;
            if (prompt != null) prompt.SetActive(true);
            RefreshVisual();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIn = false;
            if (prompt != null) prompt.SetActive(false);
        }
    }

    void Update()
    {
        if (!playerIn || coin == null || data == null) return;

        if (Input.GetKeyDown(KeyCode.V))
        {
            TryBuy();
        }
    }

    void TryBuy()
    {
        if (!coin.TrySpend(price))
        {
            // 부족 연출(살짝 흔들리거나 깜빡임 등)
            StartCoroutine(Blink());
            return;
        }

        GrantItem();
        Destroy(gameObject);
    }

    void GrantItem()
    {
        var mapRoot = player != null ? player.currentMapRoot : transform.parent;
        Instantiate(data.prefab, transform.position + Vector3.up * 0.2f, Quaternion.identity, mapRoot);
    }

    void RefreshVisual()
    {
        if (priceText != null) priceText.text = $"{price.ToString()}";

        bool canBuy = coin != null && coin.HasCoin(price);
        if (priceText != null) priceText.color = canBuy ? canBuyColor : cantBuyColor;
        if (iconRenderer != null) iconRenderer.color = canBuy ? Color.white : new Color(1, 1, 1, 0.6f);
    }

    void OnCoinChanged(int _)
    {
        RefreshVisual();
    }

    System.Collections.IEnumerator Blink()
    {
        if (priceText == null) yield break;
        var c = priceText.color;
        priceText.color = Color.red;
        yield return new WaitForSecondsRealtime(0.1f);
        priceText.color = c;
    }
}
