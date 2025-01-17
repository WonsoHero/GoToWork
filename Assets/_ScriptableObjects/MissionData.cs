using System.Collections.Generic;
using UnityEngine;

//미션이 들고 있어야 할 데이터들
[CreateAssetMenu(fileName = "MissionData", menuName = "Scriptable Objects/MissionData")]
public class MissionData : ScriptableObject
{
    // 플레이어 트랜스폼
    public Transform playerTransform;
    public List<Pose> poses;

    // 미션 오브젝트 트랜스폼
    public Transform missionObjectTransform;

    // 조작 방식

}
