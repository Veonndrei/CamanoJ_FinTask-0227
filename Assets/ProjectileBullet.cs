using UnityEngine;

public class ProjectileBullet : MonoBehaviour
{
    public float destroyAfter = 5f;

    void Start()
    {
        Destroy(gameObject, destroyAfter);
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}