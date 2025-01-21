using System.Collections.Generic;
using UnityEngine;

//미션이 들고 있어야 할 데이터들
[CreateAssetMenu(fileName = "MissionData", menuName = "Scriptable Objects/MissionData")]
public class MissionData : ScriptableObject
{
    [SerializeField] bool poseDictCreated;

    public int missionIdx;
    public bool isCleared;

    // 플레이어 트랜스폼
    public Vector3 modelPosition;
    public Quaternion modelRotation;

    public Vector3 leftHandControllerPosition;
    public Quaternion leftHandControllerRotation;

    public Vector3 rightHandControllerPosition;
    public Quaternion rightHandControllerRotation;

    //포즈
    public List<PoseName> poses;

    // 미션 오브젝트 트랜스폼
    public Vector3 missionObjPosition;
    public Quaternion missionObjRotation;

    // 조작 방식
    public HandControlMode handControlMode;
    public HandMoveAxis moveAxis;
    public HandReverse handReverse;
}