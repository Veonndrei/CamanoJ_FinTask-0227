using UnityEngine;

public class PickupDrop : MonoBehaviour
{
    [Header("References")]
    public Transform holdPoint; //Where should I hold the object?
    public Camera playerCam;

    [Header("Pickup Settings")]
    public float pickupRange = 3f; //How far can hands reach
    public LayerMask pickupMask; //Only detect pickup objects
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.Q;

    //These track the object currently held
    private Rigidbody heldRb; 
    private Collider heldCol;
    private Transform heldTf;

    public float dropForwardDistance = 1.2f; //How far in front of the camera the object is dropped
    public float dropCheckRadius = 0.35f;
    public LayerMask solidMask;  //Layers considered solid


    void Update() //Update Loop (Runs Every Frame)
    {
        if (Input.GetKeyDown(pickupKey))//Only tries pickup if nothing is already held
        {
            if (heldRb == null) TryPickup();
        }

        if (Input.GetKeyDown(dropKey))//Only drops if holding something
        {
            if (heldRb != null) Drop();
        }

        if (heldRb != null)//Object snaps to hold point
        {
            heldTf.position = holdPoint.position;
            heldTf.rotation = holdPoint.rotation;
        }
    }

    void TryPickup()//Called when pressing E
    {
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);

        float radius = 0.35f;
        if (Physics.SphereCast(ray, radius, out RaycastHit hit, pickupRange, pickupMask))
        {
            if (!hit.collider.CompareTag("Pickup")) return;

            Rigidbody rb = hit.collider.attachedRigidbody;
            if (rb == null) return;

            heldRb = rb;//This is the object I’m holding
            heldCol = hit.collider;
            heldTf = rb.transform;

            heldRb.isKinematic = true;
            heldRb.useGravity = false;

            heldTf.position = holdPoint.position;
            heldTf.rotation = holdPoint.rotation;
            heldTf.SetParent(holdPoint);
        }

    }

    void Drop()
    {
        heldTf.SetParent(null);

        //Drops in front of player and Prevents overlapwith the character
        Vector3 basePos = playerCam.transform.position + playerCam.transform.forward * dropForwardDistance;
        Vector3 safePos = FindSafeDropPosition(basePos);

        heldTf.position = safePos;

        heldRb.isKinematic = false;
        heldRb.useGravity = true;

        heldRb.linearVelocity = playerCam.transform.forward * 1.0f;

        //
        heldRb = null;
        heldCol = null;
        heldTf = null;
    }

    //the object can be safely dropped without overlapping walls
    Vector3 FindSafeDropPosition(Vector3 startPos) //
    {
        Vector3 dir = playerCam.transform.forward;
        Vector3 pos = startPos;

        const int maxSteps = 8;
        const float step = 0.25f;

        for (int i = 0; i < maxSteps; i++)
        {
            bool blocked = Physics.CheckSphere(pos, dropCheckRadius, solidMask, QueryTriggerInteraction.Ignore);
            if (!blocked) return pos;

            pos += dir * step;
        }
        return pos;
    }

}
