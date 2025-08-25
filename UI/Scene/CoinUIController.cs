using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinTxt;

    private void Start()
    {
        GameManager.Instance.playerController.coinHandler.OnCoinChanged += UpdateCoinhUI;
    }

    private void UpdateCoinhUI(int amount)
    {
        coinTxt.text = amount.ToString();
    }
}
