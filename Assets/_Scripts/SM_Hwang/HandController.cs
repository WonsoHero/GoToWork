using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HandController : MonoBehaviour
{
    [SerializeField] Rigidbody leftHand;
    [SerializeField] Rigidbody rightHand;
    [SerializeField] Image leftHandGauge;
    [SerializeField] Image rightHandGauge;
    [SerializeField] Transform leftClavicle;
    [SerializeField] Transform rightClavicle;

    Vector3 targetVelocity;
    Vector3 prevVelocity;

    [SerializeField] float moveDelay = 0.5f; //이동 시작 딜레이
    [SerializeField] float smoothFactor = 0.1f; //관성
    [SerializeField] public float multiflier = 10f; //감도 배수

    [SerializeField] HandControllerCanvas canvas;

    float mouseX;
    float mouseY;
    float mouseSpeed;
    public float maxHandSpeed = 25f;

    float mouseNotMovedTime = 0f; //마우스 이동이 없던 시간
    public float maxHandDistance=1f;
    //왼손, 오른손이 동작 중인지 체크하는 bool 변수
    private bool _isLeftHandActing = false;
    private bool _isRightHandActing = false;

    public bool isLeftHandActing { get { return _isLeftHandActing; } }
    public bool isRightHandActing { get { return _isRightHandActing; } }

    HandControlMode handControlMode;
    HandMoveAxis handMoveAxis;
    HandPower handPower;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        handControlMode = HandControlMode.None;
        handMoveAxis = HandMoveAxis.All;
        handPower= HandPower.None;
        canvas.gameObject.SetActive(false);

    }
    private void Update()
    {
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
        //테스트용 인풋 동작들
        TestParameterByInput();

        if(handControlMode!=HandControlMode.None) {
            CalcAcceleration();
            CheckMouseInput();
        }
    }
    private void FixedUpdate()
    {
        LimitHandDistance(leftHand,leftClavicle);
        LimitHandDistance(rightHand,rightClavicle);
        LimitHandPosition(leftHand);
        LimitHandPosition(rightHand);
        ControlHandPower();
        switch (handControlMode)
        {
            //손 이동 모드
            case HandControlMode.Move:
                HandMoveMode();
                break;
            //손 회전 모드
            case HandControlMode.Rotate:
                HandRotateMode();
                break;
            //조작 없음
            case HandControlMode.None:
            default:
                break;
        }
        if (!_isLeftHandActing)
        {
            StopHandMovement(leftHand);
        }
        if (!_isRightHandActing)
        {
            StopHandMovement(rightHand);
        }
    }
    void CalcAcceleration()
    {
        mouseX = 0;
        mouseY = 0;
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            // 마우스 이동 입력
            switch (handMoveAxis)
            {
                case HandMoveAxis.All:
                    mouseX = Input.GetAxis("Mouse X");
                    mouseY = Input.GetAxis("Mouse Y");
                    break;

                case HandMoveAxis.Vertical:
                    mouseY = Input.GetAxis("Mouse Y");
                    break;

                case HandMoveAxis.Horizontal:
                    mouseX = Input.GetAxis("Mouse X");
                    break;
            }
            mouseSpeed = new Vector2(mouseX, mouseY).magnitude / Time.deltaTime;
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
    void RotateHand(Rigidbody hand)
    {
        if (mouseX != 0 || mouseY != 0)
        {
            // 마우스 이동에 따른 이동 벡터 계산
            Vector3 movementVector = new Vector3(mouseX, 0f, mouseY);

            // 이동 벡터를 월드 좌표계로 변환
            Vector3 targetDir = Camera.main.transform.TransformDirection(movementVector).normalized;

            //목표 각도 계산
            float angle = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg;
            //목표 회전으로 보간
            float smoothAngle = Mathf.MoveTowardsAngle(hand.rotation.eulerAngles.y, angle, mouseSpeed * Time.deltaTime);

            canvas.ShowText(GetHandRotation().ToString());
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

    //현재 동작 중인 손의 게이지를 반환하는 함수
    public float GetHandGauge()
    {
        //이동하는 손과 힘을 주는 손이 같으면
        if (handPower == HandPower.Forward)
        {
            //왼손 동작 시 왼손 게이지 리턴
            if (_isLeftHandActing) return leftHandGauge.fillAmount;
            else return rightHandGauge.fillAmount;
        }
        //이동하는 손과 힘을 주는 손이 다르면
        else if (handPower == HandPower.Reverse)
        {
            //왼손 동작 시 오른손 게이지 리턴
            if (_isLeftHandActing) return rightHandGauge.fillAmount;
            else return leftHandGauge.fillAmount;
        }
        else return 0f;
    }
    /*현재 동작 중인 손의 회전값을 반환하는 함수*/
    public float GetHandRotation()
    {
        if (_isLeftHandActing)
        {
            return leftHand.rotation.eulerAngles.y;
        }
        else
        {
            return rightHand.rotation.eulerAngles.y;
        }
    }
    /*스페이스 바로 힘을 조절하는 함수
     해당되는 ImageGauge를 인자로 받아 어느 손에 힘을 줄지
    지정*/
    void ControlHandPower()
    {
        if (handPower == HandPower.None) return;
        void UpdateHandGauge(bool isActing, Image actingHandGauge)
        {
            if(isActing)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    actingHandGauge.fillAmount += Time.fixedDeltaTime;
                }
                else
                {
                    actingHandGauge.fillAmount -= Time.fixedDeltaTime;
                }
            }
            else
            {
                actingHandGauge.fillAmount-= Time.fixedDeltaTime;
            }
        }
        if(handPower == HandPower.Forward)
        {
            UpdateHandGauge(_isLeftHandActing,leftHandGauge);
            UpdateHandGauge(_isRightHandActing,rightHandGauge);
        }
        else if (handPower == HandPower.Reverse)
        {
            UpdateHandGauge(_isLeftHandActing, rightHandGauge);
            UpdateHandGauge(_isRightHandActing,leftHandGauge);
        }
        //게이지 범위 제한
        leftHandGauge.fillAmount = Mathf.Clamp(leftHandGauge.fillAmount, 0, 1);
        rightHandGauge.fillAmount = Mathf.Clamp(rightHandGauge.fillAmount, 0, 1);
    }
    void HandMoveMode()
    {
        if (Input.GetMouseButton(0) && !_isRightHandActing)
        {
            _isLeftHandActing = true;
            MoveHandAfterDelay(leftHand);
        }
        //오른손
        if (Input.GetMouseButton(1) && !_isLeftHandActing)
        {
            _isRightHandActing = true;
            MoveHandAfterDelay(rightHand);
        }
    }
    void HandRotateMode()
    {
        if (Input.GetMouseButton(0) && !_isRightHandActing)
        {
            _isLeftHandActing = true;
            RotateHand(leftHand);
        }
        //오른손
        if (Input.GetMouseButton(1) && !_isLeftHandActing)
        {
            _isRightHandActing = true;
            RotateHand(rightHand);
        }
    }
    /*3개의 enum인자를 통해 조작 모드를 설정하는 함수
     * 1. HandControlMode : None,Move,Rotate
     * 2. HandMoveAxis : All,Vertical,Horizontal
     * 3. HandReverse : None, Reverse
     각 항목에 대해선 HandControlMode.cs의 주석 참조*/
    public void SetHandControlMode(HandControlMode mode, HandMoveAxis moveAxis = HandMoveAxis.All,
        HandPower power=HandPower.Reverse)
    {
        handControlMode = mode;
        handMoveAxis = moveAxis;
        handPower = power;
    }
    void LimitHandDistance(Rigidbody hand, Transform clavicle)
    {
        if (clavicle == null) return;
        Vector3 leftDistance = (hand.position - clavicle.position).normalized;
        float leftCurrentDistance=Vector3.Distance(hand.position, clavicle.position);
        if (!Mathf.Approximately(leftCurrentDistance, maxHandDistance)){
            hand.MovePosition(clavicle.position + leftDistance * maxHandDistance);
        }
    }
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
    void TestParameterByInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (canvas.gameObject.activeSelf)
            {
                canvas.gameObject.SetActive(false);
            }
            else
            {
                canvas.gameObject.SetActive(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetHandControlMode(HandControlMode.Move);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetHandControlMode(HandControlMode.Rotate);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SetHandControlMode(HandControlMode.None);
        }
    }

    private void Awake()
    {
        // PlayerManager의 State Changed 콜백 등록
        PlayerManager.OnPlayerStateChanged += OnPlayerStateChanged;
        MissionManager.Instance.missionObjChanged += OnMissionChanged;
    }

    private void OnDestroy()
    {
        PlayerManager.OnPlayerStateChanged -= OnPlayerStateChanged;
        MissionManager.Instance.missionObjChanged -= OnMissionChanged;
    }

    /// <summary>
    ///  PlayerManager의 State가 변경되었을 때 호출된다
    /// </summary>
    /// <param name="param">Action에 parameter 하나만 전달되길래 두개 보내려고 대충 struct로 만듦</param>
    void OnPlayerStateChanged(PlayerStateChangedParam param)
    {
        Debug.Log($"기존상태({Enum.GetName(typeof(PlayerState), param.OldState)})에서 새로운상태({Enum.GetName(typeof(PlayerState), param.NewState)})로 전환");
    }

    void OnMissionChanged(MissionEventArgs param)
    {
        FingerIK finger = MissionManager.Instance.PlayerModel.GetComponent<FingerIK>();
        //미션 할당되면 조작모드 변경
        if (param.isAssigned)
        {
            MissionData data = param.missionOBJ.MissionData;
            //미션 데이터에서 핸드 컨트롤 모드, 움직일 축, 정역방향 불러오기
            SetHandControlMode(data.handControlMode, data.moveAxis, data.handReverse);
            if(handPower != HandPower.None)
            {
                finger.isActivated = true;
            }
        }
        //미션이 null이면 일반조작
        else
        {
            SetHandControlMode(HandControlMode.None);
            finger.isActivated = false;
        }
    }
}
