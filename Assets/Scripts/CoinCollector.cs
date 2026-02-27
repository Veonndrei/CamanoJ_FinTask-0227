using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinCollector : MonoBehaviour
{
    private int coinCount;
    public TextMeshProUGUI coinText;
    public AudioSource collectSound;

    private void Start()
    {
        UpdateCoinUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            coinCount++;

            if (collectSound != null)
                collectSound.Play();

            Destroy(other.gameObject);
            UpdateCoinUI();
        }
    }

    void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = "Coins: " + coinCount;
    }
}
