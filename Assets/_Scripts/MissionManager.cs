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
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
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
        missionObj.failed += MissionFail; //이 안에서 구독해서 미션모드로 진입 안하면 미션 실패 못함

        //미션 오브젝트 쓰는 곳에서 쓸 수 있도록 이벤트 발생
        MissionEventArgs args = new MissionEventArgs(missionObj, true); //계속 새로 만드는데 안쓰이는건 가비지 컬렉터가 알아서 회수하나??
        missionObjChanged?.Invoke(args);

        //해당 미션 조작 활성화

        Debug.Log(missionObj.name + "미션 할당됨");
    }

    void RemoveMission(int missionIdx)
    {
        Debug.Log("미션 할당 해제");

        //이벤트 구독 해제
        missionObj.succeed -= MissionComplete;
        missionObj.failed -= MissionFail;

        //캐싱 취소
        missionObj = null;
        missionData = null;

        //미션 오브젝트 쓰던 곳에서 할당 해제할 수 있도록 이벤트 발생
        MissionEventArgs args = new MissionEventArgs(missionObj, false);
        missionObjChanged?.Invoke(args);

        //해당 미션 조작 비활성화, 일반 조작모드
        
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