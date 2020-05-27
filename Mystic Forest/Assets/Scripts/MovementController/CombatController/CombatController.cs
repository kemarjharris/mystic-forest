using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhysicsZ))]
public class CombatController : MonoBehaviour
{
    PhysicsZ physics;
    public float jumpPower;
    public float moveSpeed;

    private void Awake()
    {
        physics = GetComponent<PhysicsZ>();
    }

    void FixedUpdate()
    {
        if (physics.IsGrounded && Input.GetAxis("Vertical") > 0)
        {
            // jump
            physics.SetVelocity(VectorZ.zero, jumpPower);

            // move horizontally
            float horizontal = Input.GetAxis("Horizontal");
            if (horizontal != 0)
            {
                physics.AddForce(new VectorZ(moveSpeed * horizontal, 0), 0);
            } else
            {
                IEnumerator AddHorizontalVelocity()
                {
                    int frames = 6;
                    for (int i = 0; i < frames; i++)
                    {
                        float pendingHorizontal = Input.GetAxis("Horizontal");
                        if (horizontal != 0)
                        {
                            physics.AddForce(new VectorZ(moveSpeed * horizontal, 0), 0);
                            //Debug.Break();
                            break;
                        }
                        yield return new WaitForFixedUpdate();
                    }
                }
                StartCoroutine(AddHorizontalVelocity());
            }
           
            
        }
    }
}
