using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class HandController : MonoBehaviour
{
    [SerializeField] Rigidbody leftHand;
    [SerializeField] Rigidbody rightHand;
    [SerializeField] Image leftHandGauge;
    [SerializeField] Image rightHandGauge;

    Vector3 targetVelocity;
    Vector3 prevVelocity = Vector3.zero;
    [SerializeField] float moveDelay = 0.5f; //이동 시작 딜레이
    [SerializeField] float smoothFactor = 0.1f; //관성
    [SerializeField] float multiflier = 10f; //감도 배수

    float mouseX;
    float mouseY;

    float mouseNotMovedTime = 0f; //마우스 이동이 없던 시간

    bool isLeftHandActing = false;
    bool isRightHandActing = false;

    //수치 확인용 텍스트
    [SerializeField] TextMeshProUGUI velocityTmp;
    [SerializeField] TextMeshProUGUI inputTmp;
    [SerializeField] TextMeshProUGUI multiflierTmp;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("SM_Hwang");
        }
        if (Input.GetMouseButtonUp(0))
        {
            isLeftHandActing = false;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isRightHandActing = false;
        }
        UpdateMultifly();
        CalcAcceleration();
        CheckMouseInput();
    }
    private void FixedUpdate()
    {
        //왼손
        if (Input.GetMouseButton(0) && !isRightHandActing)
        {
            isLeftHandActing = true;
            MoveHandAfterDelay(leftHand);
            if (Input.GetMouseButton(1))
            {
                rightHandGauge.fillAmount += Time.fixedDeltaTime;
            }
            else
            {
                rightHandGauge.fillAmount -= Time.fixedDeltaTime;
            }
        }

        //오른손
        if (Input.GetMouseButton(1) && !isLeftHandActing)
        {
            isRightHandActing = true;
            MoveHandAfterDelay(rightHand);
            if (Input.GetMouseButton(0))
            {
                leftHandGauge.fillAmount += Time.fixedDeltaTime;
            }
            else
            {
                leftHandGauge.fillAmount -= Time.fixedDeltaTime;
            }
        }
        if(!isLeftHandActing)
        {
            rightHandGauge.fillAmount -= Time.fixedDeltaTime;
        }
        if (!isRightHandActing)
        {
            leftHandGauge.fillAmount -= Time.fixedDeltaTime;
        }

        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            StopHandsMovement();
        }

        LimitHandPosition(leftHand);
        LimitHandPosition(rightHand);
        //게이지 범위 제한
        leftHandGauge.fillAmount = Mathf.Clamp(leftHandGauge.fillAmount, 0, 1);
        rightHandGauge.fillAmount = Mathf.Clamp(rightHandGauge.fillAmount, 0, 1);
    }
    void CalcAcceleration()
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
            inputTmp.text = "x : " + mouseX + " y : " + mouseY;
            targetVelocity = new Vector3(mouseX * Mathf.Pow(mouseX * multiflier, 2), mouseY * Mathf.Pow(mouseY * multiflier, 2), 0);
        }
    }
    void MoveHandAfterDelay(Rigidbody hand)
    {
        if (mouseX != 0 || mouseY != 0)
        {
            Debug.Log("Hand velocity: " + hand.linearVelocity.magnitude);
            velocityTmp.text = hand.linearVelocity.ToString();
            prevVelocity = Vector3.Lerp(prevVelocity, targetVelocity, smoothFactor);

            // 갱신된 속도를 코루틴으로 적용
            StartCoroutine(UpdateVelocity(hand, prevVelocity));
        }
    }

    /*MoveDelay 이후에 사용자의 마우스 움직임에 따른 velocity를 갱신한는 함수
     사용자의 마우스 컨트롤에 의한 손 이동이 즉각적이면 너무 쉽고 단조로울 거 같아 작성
     테스트를 통한 조정 필요*/
    IEnumerator UpdateVelocity(Rigidbody hand, Vector3 velocity)
    {
        yield return new WaitForSeconds(moveDelay);
        hand.linearVelocity = velocity;
    }
    /*마우스 이동이 있는 시간을 체크하고 일정 시간 동안 없으면 멈추는 함수
     시간을 체크하는 이유 : 이동이 없을 때 바로 멈추면 잠깐의 순간도 즉각적으로
    체크해서 기본적인 움직임이 뚝뚝 끊기게 됨*/
    void CheckMouseInput()
    {
        if (mouseX == 0 && mouseY == 0)
        {
            mouseNotMovedTime += Time.deltaTime;
            if (mouseNotMovedTime > 0.3f)
            {
                StopHandsMovement();
            }
        }
        else
        {
            mouseNotMovedTime = 0;
        }
    }
    /*모든 손 움직임을 멈추는 함수*/
    void StopHandsMovement()
    {
        leftHand.linearVelocity = Vector3.Lerp(leftHand.linearVelocity, Vector3.zero, smoothFactor);
        rightHand.linearVelocity = Vector3.Lerp(rightHand.linearVelocity, Vector3.zero, smoothFactor);
    }

    /*손 위치가 화면 밖으로 나가지 않게 하는 함수
     카메라 경계 부분에서 드드득 거려 개선 필요*/
    void LimitHandPosition(Rigidbody hand)
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(hand.position);
        //카메라 경계를 나가면 true
        bool isOutOfBounds = viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1;

        viewportPos.x = Mathf.Clamp(viewportPos.x, 0, 1);
        viewportPos.y = Mathf.Clamp(viewportPos.y, 0, 1);
        Vector3 worldPos = Camera.main.ViewportToWorldPoint(viewportPos);
        hand.position = worldPos;

        //isOutOfBounds면 손 정지
        if (isOutOfBounds) StopHandsMovement();
    }

    /*감도조절 함수*/
    void UpdateMultifly()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            multiflier -= 10;
            if (multiflier < 0)
            {
                multiflier = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            multiflier += 10;
        }
        multiflierTmp.text = multiflier.ToString();
    }
}
