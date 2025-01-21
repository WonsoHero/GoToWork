using System.Collections.Generic;
using UnityEngine;

public class DestructionModeSelector : MonoBehaviour
{
    [SerializeField] Collider bodyCollider;
    [SerializeField] List<Collider> handColliders;
    Rigidbody bodyRb;
    List<Rigidbody> handRbs;

    private void Awake()
    {
        bodyRb = bodyCollider.gameObject.GetComponent<Rigidbody>();

        handRbs = new List<Rigidbody>();
        foreach (Collider collider in handColliders)
        {
            handRbs.Add(collider.gameObject.GetComponent<Rigidbody>());
        }

        BodyDestructionMode();
    }

    private void OnEnable()
    {
        PlayerManager.OnPlayerStateChanged += ColliderOnOff;
    }

    private void OnDisable()
    {
        PlayerManager.OnPlayerStateChanged -= ColliderOnOff;
    }

    void ColliderOnOff(PlayerStateChangedParam param)
    {
        PlayerState state = param.NewState;
        switch (state)
        {
            case PlayerState.Normal:
                BodyDestructionMode();
                break;
            case PlayerState.Interaction:
                HandDestructionMode();
                break;
        }
    }
    void HandDestructionMode()
    {
        bodyRb.detectCollisions = false;
        bodyCollider.enabled = false;

        foreach(Collider collider in handColliders)
        {
            collider.enabled = true;
        }
        foreach(Rigidbody rb in handRbs)
        {
            rb.detectCollisions = true;
        }
    }

    void BodyDestructionMode()
    {
        foreach (Collider collider in handColliders)
        {
            collider.enabled = false;
        }
        foreach (Rigidbody rb in handRbs)
        {
            rb.detectCollisions = false;
        }

        bodyRb.detectCollisions = true;
        bodyCollider.enabled = true;
    }
}
