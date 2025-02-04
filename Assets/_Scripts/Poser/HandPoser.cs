using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPoser : MonoBehaviour
{
    [SerializeField] List<Transform> IKTargets;
    [SerializeField] MissionOBJ missionObj;

    float lerpFactor = 0.1f;
    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    private void OnEnable()
    {
        MissionManager.Instance.missionObjChanged += OnMissionChanged;
    }

    private void OnDisable()
    {
        MissionManager.Instance.missionObjChanged -= OnMissionChanged;
    }

    void OnMissionChanged(MissionEventArgs args)
    {
        if (args.isAssigned)
        {
            AssignMissionOBJ(args.missionOBJ);
        }
        else
        {
            RemoveMissionObj();
        }
    }

    //미션 매니저에서 미션 오브젝트를 받아옴
    void AssignMissionOBJ(MissionOBJ obj)
    {
        missionObj = obj;
    }

    //구독 해제
    void RemoveMissionObj()
    {
        missionObj = null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    //포즈 네임을 받아 포즈 컨테이너에서 필요한 포즈를 바로 꺼내씀
    //무슨 포즈를 쓸지는 각 미션오브젝트에서 관리해야할듯
    public void ChangePose(PoseName poseName)
    {
        //미션 안할때는 작동 안함
        if (missionObj == null) return;

        //Debug.Log("포즈 변경");
        Pose pose = PoseContainer.Instance.PoseDict[poseName];

        for (int i = 0; i < IKTargets.Count; i++)
        {
            StartCoroutine(PoseLerp(IKTargets[i], pose.Positions[i], pose.Rotations[i], lerpFactor));
            //IKTargets[i].localPosition = Vector3.Lerp(IKTargets[i].localPosition, pose.Positions[i], lerpFactor);
            //IKTargets[i].localRotation = Quaternion.Lerp(IKTargets[i].localRotation, pose.Rotations[i], lerpFactor);
        }
    }

    //서브 업데이트처럼 사용하기 위해 코루틴 돌림
    IEnumerator PoseLerp(Transform target, Vector3 position, Quaternion rotation, float lerpPercent)
    {
        while (lerpPercent < 1.1f)
        {
            yield return waitForFixedUpdate;

            //Debug.Log(lerpPercent);
            target.localPosition = Vector3.Lerp(target.localPosition, position, lerpPercent);
            target.localRotation = Quaternion.Lerp(target.localRotation, rotation, lerpPercent);
            lerpPercent += lerpFactor;
        }
    }
}
