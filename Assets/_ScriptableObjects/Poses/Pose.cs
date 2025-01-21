using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pose", menuName = "Scriptable Objects/Pose")]
public class Pose : ScriptableObject
{
    public List<Transform> transforms;
    public List<Vector3> Positions;
    public List<Quaternion> Rotations;
    public string PoseName;
}