using NUnit.Framework.Constraints;
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
    Vector3 prevVelocity;

    [SerializeField] float moveDelay = 0.5f; //이동 시작 딜레이
    [SerializeField] float smoothFactor = 0.1f; //관성
    [SerializeField] float multiflier = 10f; //감도 배수

    float mouseX;
    float mouseY;
    [SerializeField] float maxHandSpeed;

    float mouseNotMovedTime = 0f; //마우스 이동이 없던 시간

    //왼손, 오른손이 동작 중인지 체크하는 bool 변수
    private bool _isLeftHandActing = false;
    private bool _isRightHandActing = false;

    public bool isLeftHandActing { get { return _isLeftHandActing; } }
    public bool isRightHandActing { get { return _isRightHandActing; } }

    float maxHandDistance = 1f;
    internal enum HandControlMode
    {
        Move,
        Rotate,
    }
    HandControlMode handControlMode;
    //수치 확인용 텍스트
    [SerializeField] TextMeshProUGUI multiflierTmp;
    [SerializeField] TextMeshProUGUI maxHandSpeedTmp;
    [SerializeField] TextMeshProUGUI testTmp;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = false;
        handControlMode = HandControlMode.Move;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (Input.GetMouseButtonUp(0))
        {
            _isLeftHandActing = false;
            prevVelocity = Vector3.zero;
            targetVelocity = Vector3.zero;
        }
        if (Input.GetMouseButtonUp(1))
        {
            _isRightHandActing = false;
            prevVelocity = Vector3.zero;
            targetVelocity = Vector3.zero;
        }
        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            StopHandMovement(leftHand);
            StopHandMovement(rightHand);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            handControlMode = HandControlMode.Move;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            handControlMode = HandControlMode.Rotate;
        }
        UpdateMultifly();
        CalcAcceleration();
        CheckMouseInput();
        ShowInfoText();
    }
    private void FixedUpdate()
    {
        //MaintainDistance(leftHand);
        //MaintainDistance(rightHand);

        //왼손
        if (Input.GetMouseButton(0) && !_isRightHandActing)
        {
            _isLeftHandActing = true;
            if (handControlMode == HandControlMode.Move)
            {
                MoveHandAfterDelay(leftHand);
            }
            else if (handControlMode == HandControlMode.Rotate)
            {
                RotateHand(leftHand);
            }
            ControlHandPower(leftHandGauge);
        }
        //오른손
        if (Input.GetMouseButton(1) && !_isLeftHandActing)
        {
            _isRightHandActing = true;
            if (handControlMode == HandControlMode.Move)
            {
                Debug.Log("Move");
                MoveHandAfterDelay(rightHand);
            }
            else if (handControlMode == HandControlMode.Rotate)
            {
                RotateHand(rightHand);
            }
            ControlHandPower(rightHandGauge);
        }
        if (!_isLeftHandActing)
        {
            leftHandGauge.fillAmount -= Time.fixedDeltaTime;
            StopHandMovement(leftHand);
        }
        if (!_isRightHandActing)
        {
            rightHandGauge.fillAmount -= Time.fixedDeltaTime;
            StopHandMovement(rightHand);
        }

        //LimitHandPosition(leftHand);
        //LimitHandPosition(rightHand);
        //게이지 범위 제한
        leftHandGauge.fillAmount = Mathf.Clamp(leftHandGauge.fillAmount, 0, 1);
        rightHandGauge.fillAmount = Mathf.Clamp(rightHandGauge.fillAmount, 0, 1);
    }
    void CalcAcceleration()
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            // 마우스 이동 입력
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");

            // 카메라의 로컬 방향(우측, 상단) 기준으로 이동 벡터 계산
            Vector3 localMovementVector = new Vector3(mouseX, mouseY, 0f);

            // 로컬 벡터를 월드 좌표계로 변환
            Vector3 movementVector = Camera.main.transform.TransformDirection(localMovementVector);
            // 이동 속도 계산
            targetVelocity = movementVector * Mathf.Pow(new Vector2(mouseX, mouseY).magnitude * multiflier, 2);

            // 이동 속도 제한
            targetVelocity.x = Mathf.Clamp(targetVelocity.x, -maxHandSpeed, maxHandSpeed);
            targetVelocity.y = Mathf.Clamp(targetVelocity.y, -maxHandSpeed, maxHandSpeed);
            targetVelocity.z = Mathf.Clamp(targetVelocity.z, -maxHandSpeed, maxHandSpeed);
        }
    }
    void MoveHandAfterDelay(Rigidbody hand)
    {
        if (mouseX != 0 || mouseY != 0)
        {
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
    public float rotateInterporlate = 1f;
    //void RotateHand(Rigidbody hand)
    //{
    //    Debug.Log(targetVelocity);
    //    if (mouseX != 0 || mouseY != 0)
    //    {
    //        // 목표 방향을 계산 (y축 제외)
    //        Vector3 targetDir = new Vector3(targetVelocity.x, 0f, targetVelocity.z).normalized;

    //        // 목표 각도 계산 (Atan2는 -180에서 180 사이의 값을 반환)
    //        float angle = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg;
    //        // 현재 회전에서 목표 회전으로 부드럽게 보간하기
    //        float smoothAngle = Mathf.MoveTowardsAngle(hand.rotation.eulerAngles.y, angle, rotateInterporlate);
    //        //Debug.Log(smoothAngle);

    //        // 회전 적용 (Quaternion.Euler로 y축 회전만 적용)
    //        hand.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
    //    }
    //}
    void RotateHand(Rigidbody hand)
    {
        Debug.Log(targetVelocity);
        if (mouseX != 0 || mouseY != 0)
        {
            // 마우스 이동에 따른 이동 벡터 계산
            Vector3 movementVector = new Vector3(mouseX, 0f, mouseY);

            // 이동 벡터를 월드 좌표계로 변환
            Vector3 targetDir = Camera.main.transform.TransformDirection(movementVector).normalized;

            //목표 각도 계산
            //float angle = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg;
            float angle = Mathf.Atan2(mouseX,mouseY) * Mathf.Rad2Deg;

            //목표 회전으로 보간
            float smoothAngle = Mathf.MoveTowardsAngle(hand.rotation.eulerAngles.y, angle, rotateInterporlate);

            // 회전 적용 (Quaternion.Euler로 y축 회전만 적용)
            hand.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
        }
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
                StopHandMovement(leftHand);
                StopHandMovement(rightHand);
            }
        }
        else
        {
            mouseNotMovedTime = 0;
        }
    }
    /*모든 손 움직임을 멈추는 함수*/
    void StopHandMovement(Rigidbody hand)
    {
        hand.linearVelocity = Vector3.Lerp(hand.linearVelocity, Vector3.zero, smoothFactor);
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
        if (isOutOfBounds) StopHandMovement(hand);
    }

    /*감도조절 함수*/
    void UpdateMultifly()
    {
        if (SceneManager.GetActiveScene().name == "SM_Hwang")
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
            //maxSpeed Control
            if (Input.GetKeyDown(KeyCode.Q))
            {
                maxHandSpeed -= 25;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                maxHandSpeed += 25;
            }
        }
    }
    void ShowInfoText()
    {
        if (multiflierTmp != null)
        {
            multiflierTmp.text = multiflier.ToString();
        }
        if (maxHandSpeedTmp != null)
        {
            maxHandSpeedTmp.text = maxHandSpeed.ToString();
        }
        if (testTmp != null)
        {
            testTmp.text = GetHandGauge().ToString();
        }
    }
    //현재 동작 중인 손의 반대 손 게이지를 리턴하는 함수
    public float GetHandGauge()
    {
        if (_isLeftHandActing)
        {
            return rightHandGauge.fillAmount;
        }
        else
        {
            return leftHandGauge.fillAmount;
        }
    }
    /*스페이스 바로 힘을 조절하는 함수
     해당되는 ImageGauge를 인자로 받아 어느 손에 힘을 줄지
    지정*/
    void ControlHandPower(Image handGauge)
    {
        if (Input.GetKey(KeyCode.Space))
        {
            handGauge.fillAmount += Time.fixedDeltaTime;
        }
        else
        {
            handGauge.fillAmount -= Time.fixedDeltaTime;
        }
    }
    void MaintainDistance(Rigidbody hand)
    {
        float currentDistance = Vector3.Distance(hand.position, transform.position);

        if (currentDistance > maxHandDistance)
        {
            // 목표 위치로 이동하도록 조정
            Vector3 direction = (transform.position - hand.position).normalized;
            hand.position = transform.position - direction * maxHandDistance;
        }
    }
}
