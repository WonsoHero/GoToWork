using UnityEngine;

//휴대폰 알람 끄는 미션

public class MissionObj_01 : MissionOBJ
{
    [SerializeField] IPhoneDisplay display;
    [SerializeField] Destructible destructible;
    [SerializeField] Rigidbody rightHand;

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
        //슬라이드 성공하면 미션 성공
        display.suceed += OnMissionSuccess;
        //파괴가능 오브젝트가 부서지면(다수일 수 있음) 실패
        destructible.destruct += OnMissionFailed;
    }

    private void OnDisable()
    {
        display.suceed -= OnMissionSuccess;
        destructible.destruct -= OnMissionFailed;
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
        if(other.tag == "LeftHand")
        {
            enteredColliders--;

            if(enteredColliders == 0)
            {
                //벗어나면 원래 포즈 복귀
                //Debug.Log("트리거 떠남");

                inTrigger = false;
                handPoser.ChangePose(PoseName.OriginalPose);
            }
        }
    }
}
