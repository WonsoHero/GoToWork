using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionManager : MonoBehaviour
{
    public Action<MissionEventArgs> missionObjChanged;
    public Action<int> missionComplete;

    //미션에 필요한 항목들을 몽땅 들고 있음
    [SerializeField] List<MissionOBJ> missionObjects;
    Dictionary<int, MissionOBJ> missions = new Dictionary<int, MissionOBJ>();

    //미션에 사용할 게임오브젝트들
    [SerializeField] GameObject playerModel;
    [SerializeField] GameObject handControllerLeft;
    [SerializeField] GameObject handControllerRight;
    [SerializeField] GameObject leftHand;
    [SerializeField] GameObject rightHand;

    [SerializeField] GameObject missionPanel;
    [SerializeField] MissionContent missionContent;
    [SerializeField] HallwayDoor door;
    [SerializeField] GameObject clearDialogue;
    [SerializeField] GameObject failureDialogue;

    bool stageCleared = false;

    //미션에 사용할 클래스들
    [SerializeField] HandController handController;
    FixedJoint leftHandJoint;
    FixedJoint rightHandJoint;

    //지금 진행중인 미션에서 사용할것들을 캐싱
    MissionData missionData;
    MissionOBJ missionObj;

    HandPoser handPoser;

    Transform playerModelOrigin;
    Transform handControllerLeftOrigin;
    Transform handControllerRightOrigin;
    public HandPoser HandPoser { get { return handPoser; } }
    public MissionOBJ MissionOBJ { get { return missionObj; } }
    public MissionData MissionData {  get { return missionData; } }

    public HandController HandController { get { return handController; } }
    public GameObject LeftHand { get { return leftHand; } }
    public GameObject RightHand { get { return rightHand; } }
    public FixedJoint LeftHandJoint { get { return leftHandJoint; } }
    public FixedJoint RightHandJoint { get { return rightHandJoint; } }
    public GameObject PlayerModel {  get { return playerModel; } }

    public PlayerManager playerManager;

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

        playerModelOrigin = playerModel.transform;
        handControllerLeftOrigin = handControllerLeft.transform;
        handControllerRightOrigin = handControllerRight.transform;

        leftHandJoint = leftHand.GetComponent<FixedJoint>();
        rightHandJoint = rightHand.GetComponent<FixedJoint>();

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

            //미션 목표 UI에 표시
            MissionContent mc = Instantiate(missionContent);
            mc.TextChange(obj.MissionData.missionGuide);
            mc.MissionClear(obj.MissionData.isCleared);
            mc.MissionAdd(obj);
            mc.transform.parent = missionPanel.transform;
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
    public void AssignMission(int missionIdx)
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

    public void AssignMission(int missionIdx, bool loadTransform)
    {
        //캐싱
        missionObj = missions[missionIdx];
        missionData = missionObj.MissionData;

        //이벤트 구독
        missionObj.succeed += MissionComplete;

        //미션 오브젝트 쓰는 곳에서 쓸 수 있도록 이벤트 발생
        MissionEventArgs args = new MissionEventArgs(missionObj, true); //계속 새로 만드는데 안쓰이는건 가비지 컬렉터가 알아서 회수하나??
        missionObjChanged?.Invoke(args);

        if (loadTransform)
        {
            //미션에 맞게 트랜스폼 로드
            LoadTransforms();
        }

        Debug.Log(missionObj.name + "미션 할당됨");
    }

    public void RemoveMission(int missionIdx)
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
    public void RemoveMission(int missionIdx, bool loadTransform)
    {
        Debug.Log("미션 할당 해제");

        //이벤트 구독 해제
        missionObj.succeed -= MissionComplete;

        //캐싱 취소 -> 미션 0번을 대목표로 설정?
        missionObj = null;
        missionData = null; //null -> 0번 미션으로 디폴트 설정? 

        if (loadTransform)
        {
            //원래 트랜스폼으로 되돌리기
            //LoadTransforms();
            LoadOrigins();
        }

        //미션 오브젝트 쓰던 곳에서 할당 해제할 수 있도록 이벤트 발생
        MissionEventArgs args = new MissionEventArgs(missionObj, false);
        missionObjChanged?.Invoke(args);
    }

    void MissionComplete(bool success)
    {
        //Debug.Log(missionData.missionIdx + " 컴플리트");
        //완료한 미션의 인덱스를 이벤트로 쏴줌
        //missionComplete?.Invoke(missionData.missionIdx);

        missionData.isCleared = success;

        CheckComplete();
    }

    public void CheckComplete()
    {
        foreach (MissionOBJ obj in missionObjects)
        {
            if (!obj.MissionData.isCleared)
            {
                //Debug.Log("모든 미션 클리어 못함");
                return;
            }
        }

        //Debug.Log("모든 미션 클리어");
        if (!stageCleared)
        {
            stageCleared = true;
            door.OpenDoor();
            clearDialogue.SetActive(true);
        }
    }

    void MissionFail(bool fail)
    {
        failureDialogue.SetActive(true);

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
        playerModel.transform.localPosition = Vector3.zero;
        playerModel.transform.localRotation = Quaternion.identity;

        //왼손 핸드컨트롤러 트랜스폼 로드
        handControllerLeft.transform.position = handControllerLeftOrigin.position;
        handControllerLeft.transform.rotation = handControllerLeftOrigin.rotation;

        //오른손 핸드컨트롤러 트랜스폼 로드
        handControllerRight.transform.position = handControllerRightOrigin.position;
        handControllerRight.transform.rotation = handControllerRightOrigin.rotation;

        //손 포즈 복귀
        handPoser.ChangePose(PoseName.OriginalPose);

        //미션 오브젝트는 내가 건든 그대로 남아있음
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