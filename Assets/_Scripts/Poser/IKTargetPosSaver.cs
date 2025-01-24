#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IKTargetPosSaver : MonoBehaviour
{
    [SerializeField] List<Transform> Targets;
    [SerializeField] Pose pose;

    List<Vector3> TargetPositions;
    List<Quaternion> TargetAngles;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TargetPositions = new List<Vector3>(new Vector3[Targets.Count]);
        TargetAngles = new List<Quaternion>(new Quaternion[Targets.Count]);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveTransform();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadTransform();
        }
    }

    void SaveTransform()
    {
        for (int i = 0; i < Targets.Count; i++)
        {
            TargetPositions[i] = Targets[i].localPosition;
            TargetAngles[i] = Targets[i].localRotation;
        }

        if (pose != null)
        {
            pose.Positions = TargetPositions;
            pose.Rotations = TargetAngles;
        }
        else
        {
            Pose newPose = ScriptableObject.CreateInstance<Pose>();
            newPose.Positions = TargetPositions;
            newPose.Rotations = TargetAngles;

            //AssetDatabase.CreateAsset(newPose, "Assets/_ScriptableObjects/Poses/newPose.asset");
        }
        

        Debug.Log("포즈 저장됨");
    }

    void LoadTransform()
    {
        if (pose == null)
        {
            Debug.Log("포즈 비어있음");
            return;
        }
        for(int i = 0;i < Targets.Count; i++)
        {
            Targets[i].localPosition = pose.Positions[i];
            Targets[i].localRotation = pose.Rotations[i];
        }

        Debug.Log("포즈 로드됨");
    }
}

#endif