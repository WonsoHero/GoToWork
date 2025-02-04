using System;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public Action<bool> grounded;
    bool isGround = true;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Desk")
        {
            isGround = true;
            grounded?.Invoke(isGround);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Desk")
        {
            isGround = false;
            grounded?.Invoke(isGround);
        }
    }
}
