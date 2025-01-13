using UnityEngine;

/// <summary>
/// 미션에 진입하기위한 상호작용 가능한 Object들
/// </summary>
public class InteractableObject : MonoBehaviour
{
    /// <summary>
    /// Player가 해당 물체와 상호작용 했을 때 카메라가 고정될 위치
    /// </summary>
    public Transform CameraPosition;

    /// <summary>
    /// 외곽선을 적용하기 위한 Material
    /// </summary>
    [SerializeField] Material OutlineMaterial;

    /// <summary>
    /// 기본 상태 Material
    /// </summary>
    [SerializeField] Material DefaultMaterial;

    /// <summary>
    /// Player가 DetectArea 안에 있는지 여부.
    /// </summary>
    public bool IsPlayerInDetectArea { get => isPlayerInDetectArea; }
    
    private bool isPlayerInDetectArea;

    /// <summary>
    ///  강조용 외곽선을 보여준다.
    /// </summary>
    public void ShowOutline()
    {
        GetComponent<MeshRenderer>().SetMaterials(new System.Collections.Generic.List<Material> { OutlineMaterial, DefaultMaterial });
    }

    /// <summary>
    ///  강조용 외곽선을 숨긴다.
    /// </summary>
    public void HideOutline()
    {
        GetComponent<MeshRenderer>().SetMaterials(new System.Collections.Generic.List<Material> { DefaultMaterial });
    }
}
