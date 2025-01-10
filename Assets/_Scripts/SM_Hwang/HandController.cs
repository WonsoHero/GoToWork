using System.Collections;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class HandController : MonoBehaviour
{
    [SerializeField] Rigidbody leftHand;
    [SerializeField] Rigidbody rightHand;

    Vector3 targetVelocity;
    Vector3 prevVelocity = Vector3.zero;
    [SerializeField] float moveDelay = 0.5f; //이동 시작 딜레이
    [SerializeField] float smoothFactor = 0.1f; //관성
    [SerializeField] float multiflier = 10f; //감도 배수

    float mouseX;
    float mouseY;

    float mouseNotMovedTime = 0f; //마우스 이동이 없던 시간

    [SerializeField] TextMeshProUGUI velocityTmp;
    [SerializeField] TextMeshProUGUI inputTmp;
    [SerializeField] TextMeshProUGUI multiflierTmp;

    private void Start()
    {
        //Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("SM_Hwang");
        }
        UpdateMultifly();
        CheckMouseInput();
    }
    private void FixedUpdate()
    {
        CalcAcceleration();
        //?좎뙣?쎌삕
        if (Input.GetMouseButton(0))
        {
            MoveHandAfterDelay(leftHand);
        }
        //?좎룞?쇿뜝?숈삕?좎룞??
        else if (Input.GetMouseButton(1))
        {
            MoveHandAfterDelay(rightHand);
        }
        else
        {
            StopHandsMovement();
        }
        LimitHandPosition(leftHand);
        LimitHandPosition(rightHand);
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
            // `prevVelocity`瑜??꾩옱 ?먯쓽 ?띾룄濡?媛깆떊
            prevVelocity = Vector3.Lerp(prevVelocity, targetVelocity, smoothFactor);
        }
        // 갱신된 속도를 코루틴으로 적용
        StartCoroutine(UpdateVelocity(hand, prevVelocity));
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
            if (mouseNotMovedTime > 0.5f)
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
        viewportPos.x = Mathf.Clamp(viewportPos.x, 0, 1);
        viewportPos.y = Mathf.Clamp(viewportPos.y, 0, 1);
        Vector3 worldPos = Camera.main.ViewportToWorldPoint(viewportPos);
        hand.position = worldPos;
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
