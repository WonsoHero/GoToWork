using UnityEngine;

public class LaptopMission : MissionOBJ
{
    HandController handController;
    HandPoser handPoser;
    int enteredColliders = 0;
    [SerializeField] Destructible destructible;
    [SerializeField] LaptopPower laptopPower;
    [SerializeField] GameObject windowScreen;
    private void Awake()
    {
        handController = MissionManager.Instance.HandController;
        handPoser = MissionManager.Instance.HandPoser;
    }

    private void OnEnable()
    {
        //파괴가능 오브젝트가 부서지면(다수일 수 있음) 실패
        destructible.destruct += OnMissionFailed;
        laptopPower.missionSuccess += OnMissionSuccess;
        laptopPower.missionSuccess += OnPowerMissionSuccess;
    }

    private void OnDisable()
    {
        destructible.destruct -= OnMissionFailed;
        laptopPower.missionSuccess -= OnMissionSuccess;
        laptopPower.missionSuccess -= OnPowerMissionSuccess;
    }
    public void OnPowerMissionSuccess(bool success)
    {
        if (windowScreen != null)
        {
            windowScreen.SetActive(true);
        }
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
            }
        }
    }
}
