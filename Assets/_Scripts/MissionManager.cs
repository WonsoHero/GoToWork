using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionManager : MonoBehaviour
{
    //미션 진행상황도 저장해야함


    MissionOBJ mission;

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
        //씬 바뀔때 다시 등록해야함
        SceneManager.sceneLoaded += AssignMission;
        SceneManager.sceneUnloaded += RemoveMission;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AssignMission;
        SceneManager.sceneUnloaded -= RemoveMission;
    }

    void AssignMission(Scene scene, LoadSceneMode sceneMode)
    {
        mission = FindAnyObjectByType<MissionOBJ>();
        Debug.Log(mission.name + "미션 할당됨");

        AddMissionEvent();
    }

    void AddMissionEvent()
    {
        mission.succeed += MissionComplete;
        mission.failed += MissionFail;
    }

    void RemoveMission(Scene scene)
    {
        Debug.Log("미션 할당 해제");
        RemoveMissionEvent();

        mission = null;
    }

    void RemoveMissionEvent()
    {
        mission.succeed -= MissionComplete;
        mission.failed -= MissionFail;
    }

    void MissionComplete(bool success)
    {
        Debug.Log(mission.name + " 컴플리트");
    }

    void MissionFail(bool fail)
    {
        Debug.Log(mission.name + " 실패, 2초후 다시 시작");
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
