using System;
using System.Collections.Generic;
using UnityEngine;

public enum PoseName
{
    //손 펴고 있는 오리지널 포즈
    OriginalPose,

    //미션 01 - 폰 알람 미션
    IphonePointing, //왼손 검지로 가리키는 포즈
    IphoneGrab, //오른손으로 쥐고 있는 포즈
    
}

public class PoseContainer : MonoBehaviour
{
    [SerializeField] List<Pose> pose;
    [SerializeField] List<PoseName> poseNames;

    Dictionary<PoseName, Pose> poseDict;
    public Dictionary<PoseName, Pose> PoseDict {  get { return poseDict; } }

    static PoseContainer instance;
    public static PoseContainer Instance {  get { return instance; } }
    private void Awake()
    {
        instance = this;

        CreateDict(); //딕셔너리 생성
        CheckDict(); //딕셔너리 확인
    }

    //인스펙터에서 함수 실행시킬 수 있게 컨텍스트메뉴 붙였는데 왜?안됨
    [ContextMenu("CreateDict")]
    public void CreateDict()
    {
        poseDict = new Dictionary<PoseName, Pose>();

        for (int i = 0; i < pose.Count; i++)
        {
            poseDict.Add(poseNames[i], pose[i]);
        }

        Debug.Log("딕셔너리 생성됨");
    }

    [ContextMenu("CheckDict")]
    public void CheckDict()
    {
        foreach (var pose in poseDict)
        {
            Debug.Log("키 : " + pose.Key + " | 밸류 : " + pose.Value);
        }

        Debug.Log("딕셔너리 체크 완료");
    }
}