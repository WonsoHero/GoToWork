using System;
using UnityEngine;

/// <summary>
///  변기 닫혔는지 체크
/// </summary>
public class WaterClosetCloseChecker : MonoBehaviour
{
    public Action<bool> OnClosed;

    public bool IsClosed { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("WaterClosetTop"))
        {
            IsClosed = true;
            OnClosed?.Invoke(true);
        }
    }
}
