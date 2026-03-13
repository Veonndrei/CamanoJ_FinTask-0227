using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialPanel;
    public TextMeshProUGUI countdownText;
    public LevelTimer levelTimer;

    void Start()
    {
        // Freeze the game while tutorial is open
        Time.timeScale = 0f;

        // KEEP CURSOR VISIBLE
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        tutorialPanel.SetActive(false);

        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        Time.timeScale = 1f;

        countdownText.gameObject.SetActive(true);

        countdownText.text = "3";
        yield return new WaitForSeconds(1);

        countdownText.text = "2";
        yield return new WaitForSeconds(1);

        countdownText.text = "1";
        yield return new WaitForSeconds(1);

        countdownText.text = "GO!";
        yield return new WaitForSeconds(1);

        countdownText.gameObject.SetActive(false);

        // Lock cursor AFTER tutorial
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        levelTimer.StartTimer();
    }
}