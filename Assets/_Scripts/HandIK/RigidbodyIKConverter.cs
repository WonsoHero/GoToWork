using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigidbodyIKConverter : MonoBehaviour
{
    [SerializeField] List<Transform> bones;
    [SerializeField] List<Rigidbody> rigidbodies;

    private void Start()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void FixedUpdate()
    {
        for(int i = 0; i < bones.Count; i++)
        {
            Vector3 pos = bones[i].position;
            Quaternion rot = bones[i].rotation;

            rigidbodies[i].MovePosition(pos);
            rigidbodies[i].MoveRotation(rot);
        }
    }
}
