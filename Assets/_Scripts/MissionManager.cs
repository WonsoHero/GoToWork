using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionManager : MonoBehaviour
{
    public Action<MissionEventArgs> missionObjChanged;

    //미션에 필요한 항목들을 몽땅 들고 있음
    [SerializeField] List<MissionOBJ> missionObjects;
    Dictionary<int, MissionOBJ> missions = new Dictionary<int, MissionOBJ>();

    //지금 진행중인 미션에서 사용할것들을 캐싱
    [SerializeField] MissionData missionData;
    [SerializeField] MissionOBJ missionObj;

    //미션에 사용할 게임오브젝트들
    [SerializeField] GameObject playerModel;
    [SerializeField] GameObject handControllerLeft;
    [SerializeField] GameObject handControllerRight;
    HandPoser handPoser;
    public HandPoser HandPoser { get { return handPoser; } }

    Transform playerModelOrigin;
    Transform handControllerLeftOrigin;
    Transform handControllerRightOrigin;

    public MissionOBJ MissionOBJ { get { return missionObj; } }
    public MissionData MissionData {  get { return missionData; } }

    static MissionManager instance;
    public static MissionManager Instance
    {
        get
        {
            return instance;
        }
    }
    private void Awake()
    {
        //if(instance != null)
        //{
        //    Destroy(gameObject);
        //}
        //else
        //{
        //    instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}
        instance = this;

        handPoser = playerModel.GetComponent<HandPoser>();

        playerModelOrigin  = playerModel.transform;
        handControllerLeftOrigin = handControllerLeft.transform;
        handControllerRightOrigin = handControllerRight.transform;

        //이전에 플레이하다가 다시올때 남아있는거 비워줌
        missionObj = null;
        missionData = null;
    }

    private void OnEnable()
    {
        PlayerManager.OnPlayerStateChanged += OnChangePlayerState;

        //미션 오브젝트를 딕셔너리로 저장
        //이벤트 구독
        foreach (MissionOBJ obj in missionObjects)
        {
            missions.Add(obj.MissionData.missionIdx, obj);
            obj.missionStart += AssignMission;
            obj.missionStop += RemoveMission;

            //미션모드가 아니더라도 물건 부수면 미션 실패할 수 있도록 미리 구독
            obj.failed += MissionFail;
        }
    }

    private void OnDisable()
    {
        PlayerManager.OnPlayerStateChanged -= OnChangePlayerState;

        //이벤트 구독 해제
        foreach (MissionOBJ obj in missionObjects)
        {
            obj.missionStart -= AssignMission;
            obj.missionStop -= RemoveMission;

            obj.failed -= MissionFail;
        }
    }

    //플레이어 상태를 체크
    void OnChangePlayerState(PlayerStateChangedParam param)
    {
        //일반 -> 상호작용 일때
        if(param.NewState == PlayerState.Interaction)
        {

        }

        //상호작용 -> 일반 일때
        if(param.NewState == PlayerState.Normal)
        {
            //미션 중지
            missionObj.MissionStopped();
        }
    }

    //진행중인 미션 딕셔너리에서 찾아 캐싱
    void AssignMission(int missionIdx)
    {
        //캐싱
        missionObj = missions[missionIdx];
        missionData = missionObj.MissionData;

        //이벤트 구독
        missionObj.succeed += MissionComplete;

        //미션 오브젝트 쓰는 곳에서 쓸 수 있도록 이벤트 발생
        MissionEventArgs args = new MissionEventArgs(missionObj, true); //계속 새로 만드는데 안쓰이는건 가비지 컬렉터가 알아서 회수하나??
        missionObjChanged?.Invoke(args);

        //미션에 맞게 트랜스폼 로드
        LoadTransforms();


        Debug.Log(missionObj.name + "미션 할당됨");
    }

    void RemoveMission(int missionIdx)
    {
        Debug.Log("미션 할당 해제");

        //이벤트 구독 해제
        missionObj.succeed -= MissionComplete;

        //캐싱 취소 -> 미션 0번을 대목표로 설정?
        missionObj = null;
        missionData = null; //null -> 0번 미션으로 디폴트 설정? 

        //원래 트랜스폼으로 되돌리기
        //LoadTransforms();
        LoadOrigins();

        //미션 오브젝트 쓰던 곳에서 할당 해제할 수 있도록 이벤트 발생
        MissionEventArgs args = new MissionEventArgs(missionObj, false);
        missionObjChanged?.Invoke(args);
    }

    void MissionComplete(bool success)
    {
        Debug.Log(missionObj.name + " 컴플리트");
        missionData.isCleared = success;

        foreach(MissionOBJ obj in missionObjects)
        {
            if (!obj.MissionData.isCleared)
            {
                Debug.Log("모든 미션 클리어 못함");
                return;
            }
        }

        Debug.Log("모든 미션 클리어");
    }

    void MissionFail(bool fail)
    {
        Debug.Log(missionObj.name + " 실패, 2초후 다시 시작");
        StartCoroutine(RestartMission());
    }

    IEnumerator RestartMission()
    {
        yield return new WaitForSecondsRealtime(2);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void LoadTransforms()
    {
        //플레이어 모델의 트랜스폼 로드
        playerModel.transform.position = missionData.modelPosition;
        playerModel.transform.rotation = missionData.modelRotation;

        //왼손 핸드컨트롤러 트랜스폼 로드
        handControllerLeft.transform.position = missionData.leftHandControllerPosition;
        handControllerLeft.transform.rotation = missionData.leftHandControllerRotation;

        //오른손 핸드컨트롤러 트랜스폼 로드
        handControllerRight.transform.position = missionData.rightHandControllerPosition;
        handControllerRight.transform.rotation = missionData.rightHandControllerRotation;

        //미션 오브젝트 트랜스폼 로드
        missionObj.transform.position = missionData.missionObjPosition;
        missionObj.transform.rotation = missionData.missionObjRotation;
    }

    //임시 함수
    void LoadOrigins()
    {
        Debug.Log("트랜스폼 원상복구");
        //플레이어 모델의 트랜스폼 로드
        playerModel.transform.position = playerModelOrigin.position;
        playerModel.transform.rotation = playerModelOrigin.rotation;

        //왼손 핸드컨트롤러 트랜스폼 로드
        handControllerLeft.transform.position = handControllerLeftOrigin.position;
        handControllerLeft.transform.rotation = handControllerLeftOrigin.rotation;

        //오른손 핸드컨트롤러 트랜스폼 로드
        handControllerRight.transform.position = handControllerRightOrigin.position;
        handControllerRight.transform.rotation = handControllerRightOrigin.rotation;

        //미션 오브젝트는 내가 건든 그대로 남아있음
    }

    void LoadPose()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class MissionEventArgs
{
    public MissionOBJ missionOBJ { get; }
    public bool isAssigned { get; }

    public MissionEventArgs(MissionOBJ missionOBJ, bool isAssigned)
    {
        this.missionOBJ = missionOBJ;
        this.isAssigned = isAssigned;
    }
}