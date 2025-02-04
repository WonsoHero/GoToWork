using UnityEngine;

public class ToiletMission : MissionOBJ
{
    [SerializeField] Destructible destructible;

    private void OnEnable()
    {
        //파괴가능 오브젝트가 부서지면(다수일 수 있음) 실패
        destructible.destruct += OnMissionFailed;
    }

    private void OnDisable()
    {
        destructible.destruct -= OnMissionFailed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
