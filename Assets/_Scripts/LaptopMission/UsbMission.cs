using Unity.VisualScripting;
using UnityEngine;

public class UsbMission : MissionOBJ
{
    [SerializeField] Destructible destructible;
    [SerializeField] GameObject sendMailMonitor;
    private void OnEnable()
    {
        //파괴가능 오브젝트가 부서지면(다수일 수 있음) 실패
        destructible.destruct += OnMissionFailed;
    }

    private void OnDisable()
    {
        destructible.destruct -= OnMissionFailed;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("UsbPort"))
        {
            OnMissionSuccess(true);
            sendMailMonitor.SetActive(true);
        }
    }
}
