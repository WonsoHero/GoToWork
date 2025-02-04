using UnityEngine;

public class ToiletMission : MissionOBJ
{
    [SerializeField] Destructible destructible;
    [SerializeField] WaterClosetCloseChecker waterClosetCloseChecker;
    [SerializeField] FlushButton flushButton;

    private void OnEnable()
    {
        //파괴가능 오브젝트가 부서지면(다수일 수 있음) 실패
        destructible.destruct += OnMissionFailed;
        flushButton.OnPressed += OnMissionSuccess;
    }

    private void OnDisable()
    {
        destructible.destruct -= OnMissionFailed;
        flushButton.OnPressed -= OnMissionSuccess;
    }
}
