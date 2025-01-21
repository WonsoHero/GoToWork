using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HandPoser : MonoBehaviour
{
    List<Pose> poses;
    List<Transform> bones;
    List<Transform> checkPointTransforms;

    [SerializeField] RigBuilder rigBuilder;

    MultiParentConstraint leftHandHolder;
    MultiParentConstraint rightHandHolder;
    Transform leftHandBone;
    Transform rightHandBone;

    [SerializeField] GameObject missionObjPrefab;
    [SerializeField] List<Transform> IKTargets;
    [SerializeField] MissionOBJ missionObj;

    int poseIdx = 0;

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
    void Update()
    {
        
    }

    //포즈 네임을 받아 포즈 컨테이너에서 필요한 포즈를 바로 꺼내씀
    //무슨 포즈를 쓸지는 각 미션오브젝트에서 관리해야할듯
    public void ChangePose(PoseName poseName)
    {
        //미션 안할때는 작동 안함
        if (missionObj == null) return;

        Debug.Log("포즈 변경");

        Pose pose = PoseContainer.Instance.PoseDict[poseName];

        for (int i = 0; i < IKTargets.Count; i++)
        {
            IKTargets[i].localPosition = pose.Positions[i];
            IKTargets[i].localRotation = pose.Rotations[i];
        }
    }

    void HoldObject(Transform hand, MultiParentConstraint holder)
    {
        //멀티 페어런트 제약을 적용하려면 릭 루트 안에 있어야해서 손의 자식으로 넣어줌
        Transform obj = missionObjPrefab.transform;

        //미션 인덱스에 따라 체크포인트별 트랜스폼을 불러옴
        //임시로 포즈 인덱스 사용
        obj.position = checkPointTransforms[poseIdx].localPosition;
        obj.rotation = checkPointTransforms[poseIdx].localRotation;
        obj.parent = hand.transform;

        //멀티 페어런트 제약 등록
        holder.data.constrainedObject = obj;
    }

    void ReleaseObject()
    {
        //제약 해제
        leftHandHolder.data.constrainedObject = null;
        rightHandHolder.data.constrainedObject = null;

        //부모 해제
        missionObjPrefab.transform.parent = null;
    }
}
