using UnityEngine;

public class MissionObj_Spoon : MissionOBJ
{
    [SerializeField] MissionObj_Cup cup;
    [SerializeField] GameObject spoonAxis;
    bool isActivated = false;
    bool isSpaceDown = false;
    float rot;
    Quaternion spoonRot;

    FixedJoint left;
    FixedJoint right;
    Rigidbody rb;
    HandController handController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        left = MissionManager.Instance.LeftHandJoint;
        right = MissionManager.Instance.RightHandJoint;
        handController = MissionManager.Instance.HandController;
    }

    private void FixedUpdate()
    {
        if (!isActivated)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isSpaceDown = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isSpaceDown = false;
            ReleaseSpoon(right);
            ReleaseSpoon(left);
        }

        rot = handController.GetHandRotation();
        spoonRot = spoonAxis.transform.rotation;
        spoonAxis.transform.rotation = Quaternion.Euler(spoonRot.x, rot, spoonRot.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("트리거");
        if (!isActivated) return;
        Debug.Log("adsf");

        if(other.tag == "LeftHand" && isSpaceDown)
        {
            GrabSpoon(left);
        }

        if(other.tag == "RightHand" && isSpaceDown)
        {
            GrabSpoon(right);
        }
    }

    public void OnWaterFilled()
    {
        isActivated = true;
        GetComponent<Outline>().enabled = true;
        GetComponent<InteractableObject>().enabled = true;
    }

    void GrabSpoon(FixedJoint joint)
    {
        if(joint.connectedBody == null)
        {
            Debug.Log("조인트 연결함");
            //joint.connectedBody = rb;
            MissionManager.Instance.HandController.SetHandControlMode(MissionData.handControlMode);
        }
    }

    void ReleaseSpoon(FixedJoint joint)
    {
        Debug.Log("조인트 뗌");
        //joint.connectedBody = null;
        MissionManager.Instance.HandController.SetHandControlMode(MissionManager.Instance.MissionData.handControlMode);
    }
}
