using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinCollector : MonoBehaviour
{
    private int coinCount;
    public TextMeshProUGUI coinText;
    public AudioSource collectSound;

    private LevelTimer levelTimer;   // NEW

    private void Start()
    {
        UpdateCoinUI();

        levelTimer = FindObjectOfType<LevelTimer>();  // NEW
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            coinCount++;

            if (collectSound != null)
                collectSound.Play();

            if (levelTimer != null)       // NEW
                levelTimer.AddCoin();     // NEW

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