using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandController : MonoBehaviour
{
    [SerializeField] Rigidbody leftHand;
    [SerializeField] Rigidbody rightHand;

    Vector3 accelerationSpeed;
    Vector3 targetVelocity;
    [SerializeField] float moveDelay = 0.5f; //�̵� ���� ������
    [SerializeField] float smoothFactor = 0.1f; //����
    [SerializeField] float multiflier = 10f;

    float mouseX;
    float mouseY;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("SM_Hwang");
        }
    }
    void FixedUpdate()
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

            Debug.Log("X speed : " + mouseX + " Y speed : " + mouseY);
            targetVelocity = new Vector3(mouseX, mouseY, 0) * Mathf.Pow(mouseX * multiflier, 2) * Mathf.Pow(mouseY * multiflier, 2);
        }
    }
    void MoveHandAfterDelay(Rigidbody hand)
    {
        //Ŭ�� ���� ���콺 �̵� �� �� �̵� ����
        if (mouseX != 0 || mouseY != 0)
        {
            Debug.Log("Hand velocity" + hand.linearVelocity);
            Vector3 currentVelocity = hand.linearVelocity;
            Vector3 newVelocity = Vector3.Lerp(currentVelocity, targetVelocity, smoothFactor);
            hand.linearVelocity = newVelocity;

            StartCoroutine(UpdateVelocity(hand, newVelocity));
            LimitHandPosition(hand);
        }
    }
    /*MoveDelay ���Ŀ� ������� ���콺 �����ӿ� ���� velocity�� �����Ѵ� �Լ�
     * ������� ���콺 ��Ʈ�ѿ� ���� �� �̵��� �ﰢ���̸� �ʹ� ���� �����ο� �� ���� �ۼ�
     �׽�Ʈ�� ���� ���� �ʿ�*/
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
}
