using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class IPhoneDisplay : MonoBehaviour
{
    [SerializeField] GameObject display;
    [SerializeField] PlayerManager playerManager;
    [SerializeField] GameObject circle;
    [SerializeField] Transform circleLimitLeft;
    [SerializeField] Transform circleLimitRight;

    Vector3 circleOrigin;
    float playerOriginX;
    public int enteredColliers;

    public Action<bool> suceed;
    bool isSucceed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        circleOrigin = circle.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(playerManager.State.ToString());
        //if(playerManager.State == PlayerState.Interaction)
        //{
        //    display.SetActive(true);
        //}
        //else
        //{
        //    display.SetActive(false);
        //}
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            enteredColliers++;
            if(enteredColliers == 1)
            {
                //Debug.Log("터치");

                playerOriginX = other.transform.position.x;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //트리거 안에 플레이어가 들어오면 x축을 따라감
        if(other.tag == "Player" && enteredColliers >= 1)
        {
            //Debug.Log("터치중");
            float dist = other.transform.position.x - playerOriginX;

            //circle.transform.Translate();
            circle.transform.localPosition = new Vector3(
                Mathf.Clamp(circle.transform.localPosition.x + dist, circleLimitLeft.localPosition.x, circleLimitRight.localPosition.x), 
                circle.transform.localPosition.y, 
                circle.transform.localPosition.z);

            if(!isSucceed && circle.transform.localPosition.x >= circleLimitRight.localPosition.x - 0.001f)
            {
                isSucceed = true;
                suceed?.Invoke(isSucceed);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player" && !isSucceed)
        {
            enteredColliers--;
            if(enteredColliers == 0)
            {
                //Debug.Log("터치끝");

                circle.transform.localPosition = circleOrigin;
            }
        }
    }
}
