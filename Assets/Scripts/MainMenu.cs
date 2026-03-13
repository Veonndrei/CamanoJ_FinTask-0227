using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public AudioSource clickSound;
    public SceneFader sceneFader;

    public void PlayGame()
    {
        clickSound.Play();
        sceneFader.FadeToScene("level_01");
    }
}