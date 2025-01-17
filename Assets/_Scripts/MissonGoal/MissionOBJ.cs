using System;
using System.Collections;
using UnityEngine;

public class MissionOBJ : MonoBehaviour, IMissionObject
{
    public MissionData missionData;
    public Action<bool> succeed;
    public Action<bool> failed;
    public Action<bool> inTriggered;

    bool isSucceed = true;
    bool isFailed = true;

    public bool inTrigger;

    public void OnMissionSuccess()
    {
        succeed?.Invoke(isSucceed);
    }
    public void OnMissionSuccess(bool success)
    {
        Debug.Log("미션 성공 이벤트 발생");
        succeed?.Invoke(success);
    }

    public void OnMissionFailed()
    {
        failed?.Invoke(isFailed);
    }
    public void OnMissionFailed(bool fail)
    {
        Debug.Log("미션 실패 이벤트 발생");
        failed?.Invoke(fail);
    }
}
public interface IMissionObject
{
    void OnMissionSuccess();
    void OnMissionFailed();
}
