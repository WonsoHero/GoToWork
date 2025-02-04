using System;
using UnityEngine;

public class WaterTrigger : MonoBehaviour
{
    [SerializeField] MissionObj_Cup cup;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Cup" && !cup.WaterFilled)
        {
            cup.WaterFill();
        }
    }
}
