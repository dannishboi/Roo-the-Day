using UnityEngine;

public class PlayerPhysicsPush : MonoBehaviour
{
    [SerializeField] float pushPower = 3f;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        if (body == null || body.isKinematic)
            return;

        Vector3 pushDirection = new Vector3(
            hit.moveDirection.x,
            0f,
            hit.moveDirection.z
        );

        body.AddForce(
            pushDirection * pushPower,
            ForceMode.Impulse
        );

        Debug.Log("Player pushed: " + body.gameObject.name);
    }
}