using UnityEngine;

public class LaptopMission : MissionOBJ
{
    HandController handController;
    HandPoser handPoser;
    int enteredColliders = 0;

    private void Awake()
    {
        handController = MissionManager.Instance.HandController;
        handPoser = MissionManager.Instance.HandPoser;
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "LeftHand")
        {
            enteredColliders++;

            if (enteredColliders == 1)
            {
                //폰 근처로 손 가져가면 손가락 폄
                //Debug.Log("트리거됨");

                inTrigger = true;
                handPoser.ChangePose(PoseName.IphonePointing);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "LeftHand")
        {
            enteredColliders--;

            if (enteredColliders == 0)
            {
                //벗어나면 원래 포즈 복귀
                //Debug.Log("트리거 떠남");

                inTrigger = false;
                handPoser.ChangePose(PoseName.OriginalPose);
            }
        }
    }
}
