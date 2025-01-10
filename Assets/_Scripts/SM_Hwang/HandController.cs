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
    [SerializeField] float moveDelay = 0.5f; //�̵� ���� ������
    [SerializeField] float smoothFactor = 0.1f; //����
    [SerializeField] float multiflier = 10f; //���� ���

    float mouseX;
    float mouseY;

    float mouseNotMovedTime = 0f; //���콺 �̵��� ���� �ð�

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
        //�޼�
        if (Input.GetMouseButton(0))
        {
            MoveHandAfterDelay(leftHand);
        }
        //������
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
    /* ���콺 �̵�  �ӵ��� ���� ������ �ִ� �Լ�
     * mouseX,mouseY�� ��ü�� 1������ ���� ���� ������ multiflier�� ���� �� ������ ���� �ӵ� ����
     ��ġ�� �����ϸ� ������ ��ƾ� �� */
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
            Debug.Log("Hand velocity: " + hand.linearVelocity);
            velocityTmp.text = hand.linearVelocity.ToString();
            // `prevVelocity`�� ���� ���� �ӵ��� ����
            prevVelocity = Vector3.Lerp(prevVelocity, targetVelocity, smoothFactor);
        }
        // ���ŵ� �ӵ��� �ڷ�ƾ���� ����
        StartCoroutine(UpdateVelocity(hand, prevVelocity));
    }

    /*MoveDelay ���Ŀ� ������� ���콺 �����ӿ� ���� velocity�� �����Ѵ� �Լ�
     ������� ���콺 ��Ʈ�ѿ� ���� �� �̵��� �ﰢ���̸� �ʹ� ���� �����ο� �� ���� �ۼ�
     �׽�Ʈ�� ���� ���� �ʿ�*/
    IEnumerator UpdateVelocity(Rigidbody hand, Vector3 velocity)
    {
        yield return new WaitForSeconds(moveDelay);
        hand.linearVelocity = velocity;
    }
    /*���콺 �̵��� �ִ� �ð��� üũ�ϰ� ���� �ð� ���� ������ ���ߴ� �Լ�
     �ð��� üũ�ϴ� ���� : �̵��� ���� �� �ٷ� ���߸� ����� ������ �ﰢ������
    üũ�ؼ� �⺻���� �������� �Ҷ� ����� ��*/
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
    /*��� �� �������� ���ߴ� �Լ�*/
    void StopHandsMovement()
    {
        leftHand.linearVelocity = Vector3.Lerp(leftHand.linearVelocity, Vector3.zero, smoothFactor);
        rightHand.linearVelocity = Vector3.Lerp(rightHand.linearVelocity, Vector3.zero, smoothFactor);
    }

    /*�� ��ġ�� ȭ�� ������ ������ �ʰ� �ϴ� �Լ�
     ī�޶� ��� �κп��� ���� �ŷ� ���� �ʿ�*/
    void LimitHandPosition(Rigidbody hand)
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(hand.position);
        viewportPos.x = Mathf.Clamp(viewportPos.x, 0, 1);
        viewportPos.y = Mathf.Clamp(viewportPos.y, 0, 1);
        Vector3 worldPos = Camera.main.ViewportToWorldPoint(viewportPos);
        hand.position = worldPos;
    }
    /*�������� �Լ�*/
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
