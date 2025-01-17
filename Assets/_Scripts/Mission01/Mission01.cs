using System;
using UnityEngine;

//휴대폰 알람 끄는 미션

public class Mission01 : MissionOBJ
{
    [SerializeField] IPhoneDisplay display;
    [SerializeField] Destructible destructible;
    int enteredColliders = 0;

    private void Awake()
    {

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
        if (other.tag == "Player")
        {
            enteredColliders++;

            if (enteredColliders == 1)
            {
                Debug.Log("트리거됨");
                
                inTrigger = true;
                inTriggered?.Invoke(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            enteredColliders--;

            if(enteredColliders == 0)
            {
                Debug.Log("트리거 떠남");

                inTrigger = false;
                inTriggered?.Invoke(false);
            }
        }
    }
}
