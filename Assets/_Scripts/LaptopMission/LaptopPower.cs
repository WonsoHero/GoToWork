using System;
using UnityEngine;

public class LaptopPower : MonoBehaviour
{
    public Action<bool> missionSuccess;
    bool isMissionSuccess;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand"))
        {
            isMissionSuccess = true;
            missionSuccess?.Invoke(isMissionSuccess);
        }
    }
}
