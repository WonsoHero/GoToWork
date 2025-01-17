using UnityEngine;

public class HandPositioner : MonoBehaviour
{
    [SerializeField] Transform leftHandPos;
    [SerializeField] Transform rightHandPos;
    [SerializeField] Transform leftOriginPos;
    [SerializeField] Transform rightOriginPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetPosition(leftOriginPos, rightOriginPos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPosition(Transform leftTargetPos, Transform rightTargetPos)
    {
        leftHandPos.position = leftTargetPos.position;
        rightHandPos.position = rightTargetPos.position;
    }
}
