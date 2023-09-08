
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidBodyDrag : MonoBehaviour
{
    [SerializeField] float xzDrag = 0.0f;
    [SerializeField] float yDrag = 0.0f;

    Rigidbody rigidBody;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 vel = rigidBody.velocity;
        vel.x *= 1.0f - xzDrag;
        vel.y *= 1.0f - yDrag;
        vel.z *= 1.0f - xzDrag;
        rigidBody.velocity = vel;
    }
}