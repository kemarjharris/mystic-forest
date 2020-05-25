using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{

    bool IsGrounded { get => rb.position.y <= rb.position.z; }
    private Vector3 velocity = Vector3.zero;
    public float smoothness =  0.3f;
    public float speed;
    public float jumpForce;
    Rigidbody rb;

    private void Start()
    {
        transform.position = new VectorZ(transform.position.x, transform.position.z).ToVector3();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    public void Move(float horizontal, float vertical)
    {
        horizontal = Input.GetAxisRaw("Horizontal") * speed;
        vertical = Input.GetAxisRaw("Vertical") * speed;

        rb.position = Vector3.SmoothDamp(rb.position, rb.position + new VectorZ(horizontal, vertical), ref velocity, smoothness);
    }

    public void Jump()
    {
        if (IsGrounded)
        {
            rb.position = new Vector3(rb.position.x, rb.position.z + 0.01f, rb.position.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Acceleration);
            rb.useGravity = true;


            IEnumerator WhenGrounded()
            {
                while (!IsGrounded) yield return null;
                rb.useGravity = false;
                rb.position = new VectorZ(rb.position.x, rb.position.y).ToVector3();
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            }
            StartCoroutine(WhenGrounded());
        }
    }
}
