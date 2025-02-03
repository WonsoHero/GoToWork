using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseMission : MissionOBJ
{
    [SerializeField] GameObject mousePointer;
    [SerializeField] Canvas windowCanvas;
    [SerializeField] GameObject mouseGameObject;
    [SerializeField] GameObject MailImg;
    [SerializeField] Destructible destructible;

    Camera uiCamera;
    GraphicRaycaster raycaster;
    PointerEventData pointerEventData;
    List<RaycastResult> raycastResults = new List<RaycastResult>();
    Vector3 lastMousePosition;  // 이전 프레임의 마우스 오브젝트 위치
    void Start()
    {
        uiCamera = Camera.main;
        raycaster = windowCanvas.GetComponent<GraphicRaycaster>();
        pointerEventData = new PointerEventData(EventSystem.current);
        lastMousePosition=mouseGameObject.transform.position;
    }
    private void OnEnable()
    {
        //파괴가능 오브젝트가 부서지면(다수일 수 있음) 실패
        destructible.destruct += OnMissionFailed;
    }

    private void OnDisable()
    {
        destructible.destruct -= OnMissionFailed;
    }
    void Update()
    {
        if (mouseGameObject != null)
        {
            Vector3 delta = mouseGameObject.transform.position - lastMousePosition;
            delta.y = 0; // y 값은 무시
            MovePointer(delta * 2);
            lastMousePosition = mouseGameObject.transform.position;
        }
        if (Input.GetMouseButtonDown(0))
        {
            ClickButtonAtPointer();
        }
    }
    /*마우스 오브젝트 이동에 따른 캔버스의 마우스 포인터 이동*/
    void MovePointer(Vector3 delta)
    {
        Vector3 localDelta = windowCanvas.transform.InverseTransformVector(delta);

        Vector3 newPosition = mousePointer.GetComponent<RectTransform>().anchoredPosition + new Vector2(localDelta.x, localDelta.z);

        // 캔버스 내에서 위치 제한
        RectTransform canvasRect = windowCanvas.GetComponent<RectTransform>();
        newPosition.x = Mathf.Clamp(newPosition.x, -canvasRect.rect.width / 2, canvasRect.rect.width / 2);
        newPosition.y = Mathf.Clamp(newPosition.y, -canvasRect.rect.height / 2, canvasRect.rect.height / 2);

        // 적용
        mousePointer.GetComponent<RectTransform>().anchoredPosition = newPosition;
    }
    /*World position 캔버스의 버튼을 클릭하는 함수*/
    void ClickButtonAtPointer()
    {
        // 1. 포인터의 월드 좌표를 스크린 좌표로 변환
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, mousePointer.GetComponent<RectTransform>().position);

        // 2. PointerEventData에 스크린 좌표 설정
        pointerEventData.position = screenPoint;

        // 3. Raycast 실행
        raycastResults.Clear();
        raycaster.Raycast(pointerEventData, raycastResults);

        // 4. 감지된 UI 요소 중에서 버튼 클릭 처리
        foreach (var result in raycastResults)
        {
            Button button = result.gameObject.GetComponent<Button>();
            if (button != null)
            {
                OnMissionSuccess(true);
                StartCoroutine(MailAction());
                break;
            }
        }
    }
    IEnumerator MailAction()
    {
        float elapsedTime = 0f;
        float moveDuration = 3f;
        Vector3 initialPosition = MailImg.transform.position;
        float moveSpeed = 1f;

        while (elapsedTime < moveDuration)
        {
            MailImg.transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

}
