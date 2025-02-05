using System;
using UnityEngine;

public class MissionOBJ : MonoBehaviour, IMissionObject
{
    public Action<bool> succeed;
    public Action<bool> failed;
    public Action<bool> inTriggered;
    //n번 미션 시작, 중지를 알림
    public Action<int> missionStart;
    public Action<int> missionStop;

    public bool inTrigger;

    //public int missionIdx = 0;
    bool isSucceed = false;
    bool isFailed = false;

    [SerializeField] MissionData missionData;
    public MissionData MissionData { get { return missionData; } }

    public void OnMissionSuccess()
    {
        succeed?.Invoke(isSucceed);
    }
    public virtual void OnMissionSuccess(bool success)
    {
        Debug.Log("미션 성공 이벤트 발생");
        succeed?.Invoke(success);
    }

    public void OnMissionFailed()
    {
        if (missionData.isCleared) return;
        failed?.Invoke(isFailed);
    }
    public void OnMissionFailed(bool fail)
    {
        if (missionData.isCleared) return;
        Debug.Log("미션 실패 이벤트 발생");
        failed?.Invoke(fail);
    }

    public virtual void MissionStarted()
    {
        Debug.Log(missionData.missionIdx + " 시작");
        missionStart?.Invoke(missionData.missionIdx);
    }

    public void MissionStopped()
    {
        Debug.Log(missionData.missionIdx + " 중단");
        missionStop.Invoke(missionData.missionIdx);
    }
}
public interface IMissionObject
{
    void OnMissionSuccess();
    void OnMissionFailed();
}
