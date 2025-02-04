using Unity.Cinemachine;
using UnityEngine;

public class MissionObj_Spoon : MissionOBJ
{
    [SerializeField] Ground ground;
    [SerializeField] MissionObj_Cup cup;
    [SerializeField] GameObject spoonAxis;
    [SerializeField] Transform camPos;
    [SerializeField] Transform camChangePos;
    [SerializeField] Transform changeTransform;
    [SerializeField] float minPower = 0.1f;
    [SerializeField] float maxPower = 0.9f;
    [SerializeField] CinemachineCamera cam;
    [SerializeField] Material powderMat;
    [SerializeField] SpriteRenderer waterColor;
    [SerializeField] Color waterTargetColor;
    [SerializeField] Color powderTargetColor;

    bool isActivated = false;
    bool isGrounded = false;
    bool success = false;
    float rot;
    float prevRot;
    float deltaRot = 0;
    float succeessRot = 3600;
    float lerpPercent = 0;
    int triggeredCount = 0;
    Quaternion spoonRot;

    Color waterOriginalColor;
    Color powderOriginalColor;
    FixedJoint left;
    FixedJoint right;
    Rigidbody rb;
    HandController handController;
    Destructible destructible;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        destructible = cup.GetComponent<Destructible>();
        left = MissionManager.Instance.LeftHandJoint;
        right = MissionManager.Instance.RightHandJoint;
        handController = MissionManager.Instance.HandController;

        ground.grounded += OnGrounded;
        destructible.destruct += OnMissionFailed;

        waterOriginalColor = waterColor.color;
        powderOriginalColor = powderMat.color;
    }

    private void OnDisable()
    {
        ground.grounded -= OnGrounded;
        destructible.destruct -= OnMissionFailed;
    }

    private void FixedUpdate()
    {
        if (!isActivated)
        {
            return;
        }

        if (isGrounded)
        {
            float power = handController.GetHandGauge();

            if(power > minPower)
            {
                prevRot = rot;
                rot = handController.GetHandRotation();
                deltaRot += Mathf.Abs(rot - prevRot);
                spoonRot = spoonAxis.transform.rotation;
                spoonAxis.transform.rotation = Quaternion.Euler(spoonRot.x, rot, spoonRot.z);

                lerpPercent = deltaRot / succeessRot;

                powderMat.SetColor("Invisible", Color.Lerp(powderOriginalColor, powderTargetColor, lerpPercent));
                waterColor.color = Color.Lerp(waterOriginalColor, waterTargetColor, lerpPercent);
            }
            //if (Input.GetKey(KeyCode.Space))
            //{
            //    //MissionStarted();

                
            //}

            //if (Input.GetKeyUp(KeyCode.Space))
            //{
            //    //MissionStopped();
            //    //cup.MissionStarted();

            //    //MissionManager.Instance.HandController.SetHandControlMode(MissionManager.Instance.MissionData.handControlMode);
            //}

            if (power > maxPower)
            {
                destructible.Destruct();
            }
        }

        if(deltaRot > succeessRot && !success)
        {
            success = true;
            OnMissionSuccess(success);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "LeftHand" || other.tag == "RightHand")
        {
            triggeredCount++;
            if(triggeredCount > 0)
            {
                inTrigger = true;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!isActivated) return;

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "LeftHand" || other.tag == "RightHand")
        {
            triggeredCount--;
            if (triggeredCount < 1)
            {
                inTrigger = false;
            }
        }
    }

    public void OnWaterFilled()
    {
        isActivated = true;
        GetComponent<Outline>().enabled = true;
    }

    void OnGrounded(bool isGround)
    {
        isGrounded = isGround;
        if (isGrounded && isActivated)
        {
            ChangeCinemachineTarget(camChangePos);

            MissionManager.Instance.PlayerModel.transform.position = changeTransform.position;
            MissionManager.Instance.PlayerModel.transform.rotation = changeTransform.rotation;
            camPos = camChangePos;

            MissionManager.Instance.HandController.SetHandControlMode(MissionData.handControlMode, HandMoveAxis.All, HandPower.Forward);
        }
    }

    void GrabSpoon(FixedJoint joint)
    {
        if(joint.connectedBody == null)
        {
            Debug.Log("조인트 연결함");
            //joint.connectedBody = rb;
        }
    }

    void ReleaseSpoon(FixedJoint joint)
    {
        Debug.Log("조인트 뗌");
        //joint.connectedBody = null;
    }

    void ChangeCinemachineTarget(Transform transform)
    {
        var target = new CameraTarget();
        target.TrackingTarget = transform;
        cam.Target = target;
    }
}
