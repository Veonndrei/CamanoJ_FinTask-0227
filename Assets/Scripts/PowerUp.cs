using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType
    {
        SpeedBoost,
        JumpBoost
    }

    public PowerUpType powerUpType;
    public float duration = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player != null)
            {
                if (powerUpType == PowerUpType.SpeedBoost)
                {
                    player.ActivateSpeedBoost(duration);
                }
                else if (powerUpType == PowerUpType.JumpBoost)
                {
                    player.ActivateJumpBoost(duration);
                }
            }

            Destroy(gameObject);
        }
    }
}