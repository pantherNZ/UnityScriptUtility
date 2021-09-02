using UnityEngine;

[RequireComponent( typeof( Rigidbody ) )]
public class CustomGravity : MonoBehaviour
{
    public float gravityScale = 1.0f;
    Rigidbody m_rb;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 gravity = gravityScale * Vector3.up;
        m_rb.AddForce( gravity, ForceMode.Acceleration );
    }
}