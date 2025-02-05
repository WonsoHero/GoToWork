using System;
using UnityEngine;

public class MissionObj_Cup : MissionOBJ
{
    [SerializeField] Destructible destructible;
    [SerializeField] FingerIK fingerIk;
    [SerializeField] GameObject waterPlane;
    [SerializeField] MissionObj_Spoon spoon;
    [SerializeField] GameObject dialouge;

    HandController handController;
    HandPoser handPoser;
    int enteredCollider = 0;
    bool waterFilled = false;
    public bool WaterFilled {  get { return waterFilled; }}

    private void Awake()
    {
        handController = MissionManager.Instance.HandController;
        handPoser = MissionManager.Instance.HandPoser;
    }

    private void OnEnable()
    {
        destructible.destruct += OnMissionFailed;
    }

    private void OnDisable()
    {
        destructible.destruct -= OnMissionFailed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "RightHand" || other.tag == "LeftHand")
        {
            enteredCollider++;

            if(enteredCollider == 1)
            {
                inTrigger = true;
                //손가락 ik 활성화
                //fingerIk.isActivated = true;
                //컵 잡는 손모양으로 변경
                handPoser.ChangePose(PoseName.CupGrab);
            }
        }

        //바닥에 떨어지면 깨짐
        if(other.tag == "Floor")
        {
            destructible.Destruct();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "RightHand" || other.tag == "LeftHand")
        {
            enteredCollider--;

            if (enteredCollider == 0)
            {
                inTrigger = false;
                //손모양 오리지널로 돌리고 손가락 ik 비활성화
                //fingerIk.isActivated = false;
                //handPoser.ChangePose(PoseName.OriginalPose);
            }
        }
    }

    public override void MissionStarted()
    {
        base.MissionStarted();
        dialouge.SetActive(true);
    }
    //물 채움
    [ContextMenu("WaterFill")]
    public void WaterFill()
    {
        waterFilled = true;
        waterPlane.SetActive(true);
        OnMissionSuccess(true);
        spoon.OnWaterFilled();

        //상호작용 안되게 끔 왜 안꺼져
        //GetComponent<InteractableObject>().enabled = false;
    }
}
