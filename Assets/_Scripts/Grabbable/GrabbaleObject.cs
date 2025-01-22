using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class GrabbaleObject : MonoBehaviour
{
    Rigidbody rb;
    FixedJoint leftHandJoint;
    FixedJoint rightHandJoint;
    HandPoser handPoser;
    HandController handController;
    Destructible destruct;

    int enteredCollisionLeft = 0;
    int enteredCollisionRight = 0;
    float gripStrengthLeft = 0;
    float gripStrengthRight = 0;
    bool isLeftGrapped = false;
    bool isRightGrapped = false;

    public bool IsLeftGrapped { get { return isLeftGrapped; } }
    public bool IsRightGrapped { get { return isRightGrapped; } }

    public float minGripStrengthLeft = 0.3f;
    public float minGripStrengthRight = 0.3f;
    public float maxGripStrengthLeft = 0.8f;
    public float maxGripStrengthRight = 0.8f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        destruct = gameObject.GetComponent<Destructible>();

        leftHandJoint = MissionManager.Instance.LeftHandJoint;
        rightHandJoint = MissionManager.Instance.RightHandJoint;
        handPoser = MissionManager.Instance.HandPoser;
        handController = MissionManager.Instance.HandController;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "LeftHand")
        {
            enteredCollisionLeft++;
        }
        
        if (collision.gameObject.tag == "RightHand")
        {
            enteredCollisionRight++;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "LeftHand" && enteredCollisionLeft >= 1)
        {
            gripStrengthLeft = handController.GetHandGauge();

            if(gripStrengthLeft > minGripStrengthLeft)
            {
                //파괴가능한 오브젝트면서 한계 힘을 넘으면
                if(destruct != null && gripStrengthLeft > maxGripStrengthLeft)
                {
                    //물건 부숴짐
                    destruct.Destruct();
                    Debug.Log("너무 세게 잡음");
                }
                GrabObject(leftHandJoint);
                isLeftGrapped = true;
            }
            else
            {
                ReleaseObject(leftHandJoint);
                isLeftGrapped = false;
            }
        }

        if(collision.gameObject.tag == "RightHand" && enteredCollisionRight >= 1)
        {
            gripStrengthRight = handController.GetHandGauge();

            if(gripStrengthRight > minGripStrengthRight)
            {
                //파괴가능한 오브젝트면서 한계 힘을 넘으면
                if (destruct != null && gripStrengthRight > maxGripStrengthRight)
                {
                    //물건 부숴짐
                    destruct.Destruct();
                    Debug.Log("너무 세게 잡음");

                }
                GrabObject(rightHandJoint);
                isRightGrapped = true;
            }
            else
            {
                ReleaseObject(rightHandJoint);
                isRightGrapped = false;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "LeftHand")
        {
            enteredCollisionLeft--;
        }

        if (collision.gameObject.tag == "RightHand")
        {
            enteredCollisionRight--;
        }
    }

    void GrabObject(FixedJoint joint)
    {
        joint.connectedBody = rb;
        //핸드포저에서 손동작 관리
    }

    void ReleaseObject(FixedJoint joint)
    {
        joint.connectedBody = null;
    }
}
