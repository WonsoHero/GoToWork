using System;
using UnityEngine;

public class HandWashChecker : MonoBehaviour
{
    public Action<bool> OnSucceed;

    /// <summary>
    ///  몇 초간 손이 닿아야 미션 성공으로 할지
    /// </summary>
    [SerializeField] float SuccessTime = 1f;

    bool isStaying = false;
    float timer = 0f;

    // Update is called once per frame
    void Update()
    {
        if(isStaying)
        {
            //Debug.Log("IsStaying : " + timer.ToString());
            timer += Time.deltaTime;

            if (timer > SuccessTime) {
                OnSucceed?.Invoke(true);
                Destroy(this);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("LeftHand") || other.gameObject.CompareTag("RightHand"))
        {
            isStaying = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("LeftHand") || other.gameObject.CompareTag("RightHand"))
        {
            isStaying = false;
        }
    }
}
