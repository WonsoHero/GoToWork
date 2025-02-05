using System;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class GrabbaleObject : MonoBehaviour
{
    [SerializeField] PoseName grabPose;
    bool isSimpleMode = false;

    Rigidbody rb;
    FixedJoint leftHandJoint;
    FixedJoint rightHandJoint;
    HandPoser handPoser;
    HandController handController;
    Destructible destruct;

    int enteredCollision = 0;
    int enteredCollisionLeft = 0;
    int enteredCollisionRight = 0;
    float gripStrengthLeft = 0;
    float gripStrengthRight = 0;
    float gripStrength = 0;
    bool isLeftGrapped = false;
    bool isRightGrapped = false;
    bool isSpaceDown = false;

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

    private void FixedUpdate()
    {
        gripStrength = handController.GetHandGauge();

        if (isSpaceDown)
        {
            if (isLeftGrapped)
            {
                //파괴가능한 오브젝트면서 한계 힘을 넘으면
                if (destruct != null && gripStrength > maxGripStrengthLeft)
                {
                    //물건 부숴짐
                    destruct.Destruct();
                    Debug.Log("너무 세게 잡음");
                }
            }

            if (isRightGrapped)
            {
                //파괴가능한 오브젝트면서 한계 힘을 넘으면
                if (destruct != null && gripStrength > maxGripStrengthRight)
                {
                    //물건 부숴짐
                    destruct.Destruct();
                    Debug.Log("너무 세게 잡음");
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "LeftHand")
        {
            enteredCollision++;
            enteredCollisionLeft++;
        }
        
        if (collision.gameObject.tag == "RightHand")
        {
            enteredCollision++;
            enteredCollisionRight++;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (isSimpleMode)
        {
            if (collision.gameObject.tag == "LeftHand" && enteredCollisionLeft >= 1)
            {

            }
            if (collision.gameObject.tag == "RightHand" && enteredCollisionRight >= 1)
            {

            }
            return;
        }

        if (collision.gameObject.tag == "LeftHand" && enteredCollisionLeft >= 1)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isSpaceDown = true;
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                isSpaceDown = false;
            }

            gripStrengthLeft = handController.GetHandGauge();

            if(isSpaceDown && gripStrengthLeft > minGripStrengthLeft)
            {
                
                if (!isLeftGrapped)
                {
                    GrabObject(leftHandJoint);
                    isLeftGrapped = true;
                }
            }
            else
            {
                ReleaseObject(leftHandJoint);
                isLeftGrapped = false;
            }
        }

        if(collision.gameObject.tag == "RightHand" && enteredCollisionRight >= 1)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isSpaceDown = true;
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                isSpaceDown = false;
            }

            gripStrengthRight = handController.GetHandGauge();

            if(isSpaceDown && gripStrengthRight > minGripStrengthRight)
            {
                if (!isRightGrapped)
                {
                    GrabObject(rightHandJoint);
                    isRightGrapped = true;
                }
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
            enteredCollision--;
            enteredCollisionLeft--;
        }

        if (collision.gameObject.tag == "RightHand")
        {
            enteredCollision--;
            enteredCollisionRight--;
        }
    }

    void GrabObject(FixedJoint joint)
    {
        joint.connectedBody = rb;

        Debug.Log(gameObject + "잡음");
        //잡으면 핸드포저에서 손동작 변경
        //handPoser.ChangePose(grabPose);
    }

    void ReleaseObject(FixedJoint joint)
    {
        joint.connectedBody = null;

        //안잡고 있으면 오리지널 포즈로 복귀
        //handPoser.ChangePose(PoseName.OriginalPose);
    }
}
