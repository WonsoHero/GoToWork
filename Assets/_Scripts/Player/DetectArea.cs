using UnityEngine;

/// <summary>
///  근처에 있고, 보이는 Interactable을 감지하여 외곽선을 하이라이트 적용
/// </summary>
public class DetectArea : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Outline>(out var obj))
        {
            Debug.Log("OnTriggerEnter : " + other.name);
            obj.OutlineColor = Color.white;
            obj.OutlineMode = Outline.Mode.OutlineAll;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Outline>(out var obj))
        {
            Debug.Log("OnTriggerExit : " + other.name);
            obj.OutlineColor = Color.clear;
            //obj.OutlineMode = Outline.Mode.SilhouetteOnly;
        }
    }
}
