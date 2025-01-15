using System;
using System.Collections;
using UnityEngine;

public class MissionOBJ : MonoBehaviour
{
    public Action<bool> achieved;
    public Transform holdPosition;

    bool isAchieved = false;
    float stayTime = 0;
    float goalTime = 2.0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player" && !isAchieved)
        {
            stayTime += Time.fixedDeltaTime;
            if(stayTime > goalTime)
            {
                isAchieved = true;
                DoSomething();
                stayTime = 0;
            }
        }
    }

    void DoSomething()
    {
        achieved?.Invoke(isAchieved);
    }
}
