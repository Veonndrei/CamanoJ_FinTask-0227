using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    [Header("Ground Check")]
    public float rayDistance = 0.4f;
    public LayerMask groundMask;

    public bool IsGrounded { get; private set; }
    public RaycastHit groundHit;   // ✅ DECLARED HERE (lowercase g)

    void Update()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        IsGrounded = Physics.Raycast(
            origin,
            Vector3.down,
            out groundHit,
            rayDistance,
            groundMask
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawLine(origin, origin + Vector3.down * rayDistance);
    }
}