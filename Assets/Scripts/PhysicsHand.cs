using UnityEngine;

public class PhysicsHandFollower : MonoBehaviour
{
    [SerializeField] Transform targetHand;
    [SerializeField] float pushStrength = 8f;
    [SerializeField] float maxPushForce = 20f;

    private Rigidbody rb;
    private Vector3 previousPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        previousPosition = targetHand.position;
    }

    void FixedUpdate()
    {
        rb.MovePosition(targetHand.position);
        rb.MoveRotation(targetHand.rotation);

        previousPosition = targetHand.position;
    }

    void OnCollisionStay(Collision collision)
    {
        Rigidbody body = collision.rigidbody;

        if (body == null || body.isKinematic)
            return;

        Vector3 handVelocity =
            rb.linearVelocity;

        Vector3 force =
            handVelocity * pushStrength;

        force = Vector3.ClampMagnitude(
            force,
            maxPushForce
        );

        if (collision.contactCount > 0)
        {
            body.AddForceAtPosition(
                force,
                collision.GetContact(0).point,
                ForceMode.Force
            );
        }
    }
}