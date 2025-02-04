using UnityEngine;

/// <summary>
///  변기 열고 닫히는 것
/// </summary>
public class WaterClosetController : MonoBehaviour
{
    public float pushForce = 15f;
    private Rigidbody rb;
    private HingeJoint hinge;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        hinge = GetComponent<HingeJoint>();
        //rb.centerOfMass = new Vector3(0, 0.922f, -0.364f);
        rb.centerOfMass = hinge.anchor;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("LeftHand") || collision.gameObject.CompareTag("RightHand"))
        {
            Debug.Log("물체에 손충돌");
            foreach (ContactPoint contact in collision.contacts)
            {
                // 힘 적용
                Vector3 forceDirection = contact.normal * 1;
                rb.AddForceAtPosition(forceDirection * pushForce, contact.point, ForceMode.Force);
            }
        }
    }
}
