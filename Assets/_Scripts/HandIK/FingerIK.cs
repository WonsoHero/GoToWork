using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FingerIK : MonoBehaviour
{
    [SerializeField] List<ChainIKConstraint> constraints;

    public float currentWeight = 0;
    public float targetWeight = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            foreach(ChainIKConstraint constraint in constraints)
            {
                if (targetWeight < 1)
                {
                    targetWeight += 0.05f;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            foreach(ChainIKConstraint constraint in constraints)
            {
                if (targetWeight > 0)
                {
                    targetWeight -= 0.05f;
                }
            }
        }

        foreach(ChainIKConstraint constraint in constraints)
        {
            currentWeight = constraint.weight;
            constraint.weight = Mathf.Lerp(currentWeight, targetWeight, 0.1f);
        }
    }
}
