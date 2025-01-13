using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Outline 그리기용 쉐이더
/// </summary>
public class OutlineScript : MonoBehaviour
{
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private float outlineScaleFactor;
    [SerializeField] private Color outlineColor;
    private Renderer outlineRenderer;

    void Start()
    {
        outlineRenderer = CreateOutline(outlineMaterial, outlineScaleFactor, outlineColor);
        outlineRenderer.enabled = true;
    }

    Renderer CreateOutline(Material outlineMat, float scaleFactor, Color color)
    {
        GameObject outlineObject = Instantiate(gameObject, transform.position, transform.rotation, transform);
        Renderer renderer = outlineObject.GetComponent<Renderer>();

        renderer.material = outlineMat;
        renderer.material.SetColor("_OutlineColor", color);
        renderer.material.SetFloat("_Scale", scaleFactor);
        renderer.shadowCastingMode = ShadowCastingMode.Off;

        outlineObject.GetComponent<OutlineScript>().enabled = false;
        outlineObject.GetComponent<Collider>().enabled = false;

        renderer.enabled = false;

        return renderer;
    }
}
