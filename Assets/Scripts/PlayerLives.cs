using UnityEngine;

public class PlayerLives : MonoBehaviour
{
    public int lives = 4;

    public void TakeDamage(int damage)
    {
        lives -= damage;

        Debug.Log("Player Lives: " + lives);

        if (lives <= 0)
        {
            Debug.Log("Player Dead");
        }
    }
}