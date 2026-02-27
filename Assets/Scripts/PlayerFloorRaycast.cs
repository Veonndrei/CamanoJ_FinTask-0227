using UnityEngine;

public class PlayerFloorRaycast : MonoBehaviour
{
    public float checkDist = 0.3f;
    public LayerMask groundMask;

    void Update()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, checkDist, groundMask))
        {
            Debug.Log("Standing on: " + hit.collider.name);
        }
    }
}