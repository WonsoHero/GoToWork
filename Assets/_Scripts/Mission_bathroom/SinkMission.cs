using UnityEngine;

public class SinkMission : MissionOBJ
{
    [SerializeField] Destructible destructible;
    [SerializeField] HandWashChecker handWashChecker;

    private void OnEnable()
    {
        //파괴가능 오브젝트가 부서지면(다수일 수 있음) 실패
        destructible.destruct += OnMissionFailed;
        handWashChecker.OnSucceed += OnMissionSuccess;
    }

    private void OnDisable()
    {
        destructible.destruct -= OnMissionFailed;
        handWashChecker.OnSucceed -= OnMissionSuccess;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
