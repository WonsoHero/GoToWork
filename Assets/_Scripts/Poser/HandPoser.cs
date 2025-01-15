using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HandPoser : MonoBehaviour
{
    [SerializeField] GameObject missionObjPrefab;

    [SerializeField] List<Pose> poses;
    [SerializeField] List<Transform> bones;
    [SerializeField] List<Transform> checkPointTransforms;

    [SerializeField] RigBuilder rigBuilder;

    [SerializeField] MultiParentConstraint leftHandHolder;
    [SerializeField] MultiParentConstraint rightHandHolder;
    [SerializeField] Transform leftHandBone;
    [SerializeField] Transform rightHandBone;

    MissionOBJ missionObj;

    int poseIdx = 0;

    private void Awake()
    {
        missionObj = missionObjPrefab.GetComponent<MissionOBJ>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnEnable()
    {
        missionObj.achieved += OnAchieved;
    }

    private void OnDisable()
    {
        missionObj.achieved -= OnAchieved;
    }

    void OnAchieved(bool isAchieved)
    {
        if (isAchieved)
        {
            ChangePose();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            poseIdx++;
            poseIdx %= poses.Count;
            Debug.Log("현재 포즈 : " +  poseIdx + " 번");
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            poseIdx--;
            poseIdx %= poses.Count;
            Debug.Log("현재 포즈 : " + poseIdx + " 번");
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            ChangePose();
            Debug.Log("현재 포즈 : " + poseIdx + " 번");
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            HoldObject(leftHandBone, leftHandHolder);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            HoldObject(rightHandBone, rightHandHolder);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReleaseObject();
        }
    }

    //추후 게임 매니저에서 미션별 인덱스 받아와 포즈 관리
    //임시로 포즈 인덱스 사용
    public void ChangePose()
    {
        Debug.Log("포즈 변경");
        //-1은 포즈가 없는 기본 상태, 인덱스 오류 방지를 위해 리턴
        if (poseIdx == -1) return;

        //리그 빌더가 본을 통제하고 있어 트랜스폼이 안먹혀 잠깐 껐다 킴
        rigBuilder.enabled = false;
        
        Pose targetPose = poses[poseIdx];
        for(int i = 0; i < bones.Count; i++)
        {
            bones[i].transform.position = targetPose.transforms[i].position;
            bones[i].transform.rotation = targetPose.transforms[i].rotation;
        }

        rigBuilder.enabled = true;
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
