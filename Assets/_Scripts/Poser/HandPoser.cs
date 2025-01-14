using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HandPoser : MonoBehaviour
{
    [SerializeField] MissionOBJ missionObj;
    [SerializeField] List<Pose> poses;
    [SerializeField] List<Transform> bones;
    [SerializeField] RigBuilder rigBuilder;
    int poseIdx = 0;

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
    }

    //추후 게임 매니저에서 미션별 인덱스 받아와 포즈 관리 
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
}
