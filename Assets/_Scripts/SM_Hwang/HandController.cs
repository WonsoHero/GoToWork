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
    Vector3 prevVelocity =Vector3.zero;
    [SerializeField] float moveDelay = 0.5f; //이동 시작 딜레이
    [SerializeField] float smoothFactor = 0.1f; //관성
    [SerializeField] float multiflier = 10f;

    float mouseX;
    float mouseY;

    [SerializeField] TextMeshProUGUI velocityTmp;
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
    }
    private void FixedUpdate()
    {
        CalcAcceleration();
        //왼손
        if (Input.GetMouseButton(0))
        {
            MoveHandAfterDelay(leftHand);
        }
        //오른손
        else if (Input.GetMouseButton(1))
        {
            MoveHandAfterDelay(rightHand);
        }
        else
        {
            leftHand.linearVelocity = Vector3.Lerp(leftHand.linearVelocity, Vector3.zero, smoothFactor);
            rightHand.linearVelocity = Vector3.Lerp(rightHand.linearVelocity, Vector3.zero, smoothFactor);
        }
    }
    /* 마우스 이동  속도에 따른 가속을 주는 함수
     * mouseX,mouseY가 대체로 1이하의 값이 나와 적절한 multiflier를 곱한 후 제곱을 통해 속도 증가
     수치를 조절하며 조작을 깎아야 함 */
    void CalcAcceleration()
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");

            targetVelocity = new Vector3(mouseX * Mathf.Pow(mouseX * multiflier, 2), mouseY* Mathf.Pow(mouseY * multiflier, 2), 0)  ;
        }
    }
    void MoveHandAfterDelay(Rigidbody hand)
    {
        if (mouseX != 0 || mouseY != 0)
        {
            Debug.Log("Hand velocity: " + hand.linearVelocity);
            velocityTmp.text = hand.linearVelocity.ToString();
            // `prevVelocity`를 현재 손의 속도로 갱신
            prevVelocity = Vector3.Lerp(prevVelocity, targetVelocity, smoothFactor);

            // 갱신된 속도를 코루틴으로 적용
            StartCoroutine(UpdateVelocity(hand, prevVelocity));
            LimitHandPosition(hand);
        }
        //else
        //{
        //    hand.linearVelocity = Vector3.Lerp(hand.linearVelocity, Vector3.zero, smoothFactor);
        //}
    }

    /*MoveDelay 이후에 사용자의 마우스 움직임에 따른 velocity를 갱신한는 함수
     * 사용자의 마우스 컨트롤에 의한 손 이동이 즉각적이면 너무 쉽고 단조로울 거 같아 작성
     테스트를 통한 조정 필요*/
    IEnumerator UpdateVelocity(Rigidbody hand, Vector3 velocity)
    {
        yield return new WaitForSeconds(moveDelay);
        hand.linearVelocity = velocity;
    }
    void LimitHandPosition(Rigidbody hand)
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(hand.position);
        viewportPos.x = Mathf.Clamp(viewportPos.x, 0, 1);
        viewportPos.y = Mathf.Clamp(viewportPos.y, 0, 1);
        Vector3 worldPos = Camera.main.ViewportToWorldPoint(viewportPos);
        hand.position = worldPos;
    }
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
