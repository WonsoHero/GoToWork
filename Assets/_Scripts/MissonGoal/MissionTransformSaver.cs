using UnityEngine;

public class MissionTransformSaver : MonoBehaviour
{
    [SerializeField] MissionData data;

    [SerializeField] GameObject playerModel;
    [SerializeField] GameObject missionObjPrefab;
    [SerializeField] GameObject handControllerLeft;
    [SerializeField] GameObject handControllerRight;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            SaveTransforms();
        }
    }

    void SaveTransforms()
    {
        if(data == null)
        {
            Debug.Log("미션 데이터 할당 안됨");
            return;
        }

        //플레이어 모델의 트랜스폼 저장
        data.modelPosition = playerModel.transform.position;
        data.modelRotation = playerModel.transform.rotation;

        //왼손 핸드컨트롤러 트랜스폼 저장
        data.leftHandControllerPosition = handControllerLeft.transform.position;
        data.leftHandControllerRotation = handControllerLeft.transform.rotation;

        //오른손 핸드컨트롤러 트랜스폼 저장
        data.rightHandControllerPosition = handControllerRight.transform.position;
        data.rightHandControllerRotation = handControllerRight.transform.rotation;

        //미션 오브젝트 트랜스폼 저장
        data.missionObjPosition = missionObjPrefab.transform.position;
        data.missionObjRotation = missionObjPrefab.transform.rotation;

        Debug.Log("트랜스폼 저장완료");
    }
}
