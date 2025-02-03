using StarterAssets;
using System;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
///  현재 Player의 상태,
///  Interaction, InCutScene 상태에서는 이동을 막는다.
///  [ThirdPersonController.BlockInput()] 사용하기
/// </summary>
public enum PlayerState
{
    Normal, // 일반적인 상태
    Interaction, // 상호작용 모드에 있는 상태. 이동을 막는다.
    InCutScene // 컷씬이 나오고 있는 상태. 이동을 막는다.
}

/// <summary>
/// 플레이어의 이동을 제외한 것들을 관리하는 클래스
/// 상호작용, 상태 등
/// </summary>
public class PlayerManager : MonoBehaviour
{
    /// <summary>
    /// 모델링 교체용 팬티 모델
    /// </summary>
    [SerializeField] GameObject PantsModel;

    /// <summary>
    /// 모델링 교체용 수트 모델
    /// </summary>
    [SerializeField] GameObject SuitModel;

    /// <summary>
    ///  상호작용 모드 탈출할 때 다시 원래자리로 되돌릴 카메라 위치
    /// </summary>
    [SerializeField] Transform OriginalCameraTarget;

    /// <summary>
    ///  상호작용 가능한 거리
    /// </summary>
    [SerializeField] float InteractionRange = 2f;

    private PlayerState state;
    private InteractableObject selectedInteractableObject;

    /// <summary>
    ///  현재 Player의 State를 가져온다.
    /// </summary>
    public PlayerState State { get => state; }

    /// <summary>
    /// 현재 화면 중앙 근처에 있는 InteractableObject
    /// Update에서 화면 중앙에서 Raycast 해서 선택한다
    /// 이것이 존재해야 EnterInteraction 가능
    /// </summary>
    public InteractableObject SelectedInteractableObject { get; }

    /// <summary>
    ///  외부에서 구독해서 사용 할 PlayerState 변경 콜백
    /// </summary>
    public static Action<PlayerStateChangedParam> OnPlayerStateChanged;

    void Start()
    {
        state = PlayerState.Normal;
    }

    /// <summary>
    ///  상태 변경
    /// </summary>
    /// <param name="oldState">기존 상태</param>
    /// <param name="newState">변경된 새로운 상태</param>
    void ChangeState(PlayerState newState)
    {
        var param = new PlayerStateChangedParam(state, newState);

        state = newState;
        OnPlayerStateChanged?.Invoke(param);
    }

    /// <summary>
    ///  상호작용 모드에 진입한다
    /// </summary>
    /// <param name="interactableObject"></param>
    void EnterInteraction(InteractableObject interactableObject)
    {
        Debug.Log("Entered Interaction mode");
        PlayerTestDebug.Instance.ShowAlertText(Strings.PlayerInInteractionMode);
        PlayerTestDebug.Instance.ChangeInformationText(String.Empty);

        var thirdPersonController = GetComponentInChildren<ThirdPersonController>();
        thirdPersonController.BlockInput(true);
        thirdPersonController.StopAnimation();

        ChangeState(PlayerState.Interaction);

        interactableObject.GetComponent<MissionOBJ>().MissionStarted(); //상호작용한 물체의 미션이 뭔지 체크
        interactableObject.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineHidden;
        ChangeCinemachineTarget(interactableObject.CameraPosition);
    }

    /// <summary>
    /// 상호작용 모드에서 나온다
    /// </summary>
    void ExitInteraction()
    {
        Debug.Log("Exit Interaction");
        PlayerTestDebug.Instance.ChangeInformationText(String.Empty);
        var thirdPersonController = GetComponentInChildren<ThirdPersonController>();
        thirdPersonController.BlockInput(false);
        thirdPersonController.ResumeAnimation();

        ChangeState(PlayerState.Normal);
        
        ChangeCinemachineTarget(OriginalCameraTarget);
    }

    private void Update()
    {
        DetectInteractableRaycast();
        HandleInput();
    }

    /// <summary>
    ///  키 입력 처리
    /// </summary>
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if(state == PlayerState.Interaction)
            {
                ExitInteraction();
                return;
            }

            if(selectedInteractableObject != null)
            {
                EnterInteraction(selectedInteractableObject);
            }
        }

#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.Z))
        {
            ChangeModelToPants();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            ChangeModelToSuit();
        }
#endif

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(!PauseMenu.Instance.PauseMenuActiveSelf)
            {
                PauseMenu.Instance.ShowPauseMenu();
                var thirdPersonController = GetComponentInChildren<ThirdPersonController>();
                thirdPersonController.BlockInput(true);

            } else
            {
                PauseMenu.Instance.HidePauseMenu();
                if(state == PlayerState.Normal)
                {
                    var thirdPersonController = GetComponentInChildren<ThirdPersonController>();
                    thirdPersonController.BlockInput(false);
                }
            }
        }
    }


    /// <summary>
    ///  화면 중앙 (크로스헤어?)에 Interactable 이 있는지 감지
    /// </summary>
    void DetectInteractableRaycast()
    {
        // 일반 상태일 때만 작동
        if (state == PlayerState.Interaction || state == PlayerState.InCutScene) return;

        // 화면 중앙에서 나가는 ray
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        RaycastHit hit;

        // InteractionRange 거리만큼 Raycast
        if (Physics.Raycast(ray, out hit, InteractionRange, LayerMask.GetMask("Interactable")))
        {
            // InteractableObject가 감지되면
            if (hit.collider.gameObject.TryGetComponent<InteractableObject>(out var obj))
            {
                PlayerTestDebug.Instance.ChangeInformationText(Strings.PlayerEnterInteractionMode);
                selectedInteractableObject = obj;
                return;
            }
        }

        PlayerTestDebug.Instance.ChangeInformationText(String.Empty);
        selectedInteractableObject = null;
    }

    /// <summary>
    /// 시네머신 카메라의 Tracking Target을 변경한다.
    /// </summary>
    void ChangeCinemachineTarget(Transform transform)
    {
        var target = new CameraTarget();
        target.TrackingTarget = transform;
        GetComponentInChildren<CinemachineCamera>().Target = target;
    }

    public void ChangeModelToPants()
    {
        //if(currentModel != null) Destroy(currentModel);
        //currentModel = Instantiate(PantsModel, transform);
        PantsModel.SetActive(true);
        PantsModel.transform.SetAsFirstSibling();
        SuitModel.SetActive(false);
    }

    public void ChangeModelToSuit()
    {
        //if (currentModel != null) Destroy(currentModel);
        //currentModel = Instantiate(SuitModel, transform);
        SuitModel.SetActive(true);
        SuitModel.transform.SetAsFirstSibling();
        PantsModel.SetActive(false);
    }
}
