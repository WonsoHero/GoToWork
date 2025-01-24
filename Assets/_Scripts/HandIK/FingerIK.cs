using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class FingerIK : MonoBehaviour
{
    [SerializeField] Rig leftFingerRig;
    [SerializeField] Rig rightFingerRig;
    [SerializeField] HandController controller;
    TwoBoneIKConstraintJob job;

    public float targetWeight = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveFinger();
    }

    void MoveFinger()
    {
        if (controller.isLeftHandActing)
        {
            targetWeight = controller.GetHandGauge();
            leftFingerRig.weight = Mathf.Lerp(leftFingerRig.weight, targetWeight, 0.1f);
        }
        else
        {
            leftFingerRig.weight = Mathf.Lerp(leftFingerRig.weight, 0, 0.1f);
        }

        if(controller.isRightHandActing)
        {
            targetWeight = controller.GetHandGauge();
            rightFingerRig.weight = Mathf.Lerp(rightFingerRig.weight, targetWeight, 0.1f);
        }
        else
        {
            rightFingerRig.weight = Mathf.Lerp(rightFingerRig.weight, 0, 0.1f);
        }
    }
}
