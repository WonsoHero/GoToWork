using Unity.VisualScripting;
using UnityEngine;

public class UsbMission : MissionOBJ
{
    [SerializeField] Destructible destructible;
    [SerializeField] GameObject sendMailMonitor;
    [SerializeField] Transform usbPort;
    private void OnEnable()
    {
        //파괴가능 오브젝트가 부서지면(다수일 수 있음) 실패
        destructible.destruct += OnMissionFailed;
    }

    private void OnDisable()
    {
        destructible.destruct -= OnMissionFailed;
    }
    public override void OnMissionSuccess(bool success)
    {
        base.OnMissionSuccess(success);
        MissionData.isCleared = true;
        MissionManager.Instance.CheckComplete();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("UsbPort"))
        {
            Debug.Log("Trigger USB Port");
            OnMissionSuccess(true);
            other.GetComponent<Outline>().enabled = false;
            FixUsbToLaptop(other.transform);
            sendMailMonitor.SetActive(true);
        }
    }
    void FixUsbToLaptop(Transform usbPortTransform)
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
        transform.rotation = Quaternion.Euler(90, 0, 90);
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        transform.SetParent(usbPortTransform);
        transform.localPosition= Vector3.zero;
    }
}
