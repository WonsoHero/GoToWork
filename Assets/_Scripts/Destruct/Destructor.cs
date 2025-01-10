using System.Collections.Generic;
using UnityEngine;

public class Destructor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        //List<ContactPoint> contactPoints = new List<ContactPoint>();
        //collision.GetContacts(contactPoints);

        ////if (collision.transform.gameObject.TryGetComponent(out Destructible d))
        ////{
        ////    foreach (ContactPoint contactPoint in contactPoints)
        ////    {
        ////        Debug.Log("콜리전 엔터 : " + contactPoint.impulse);
        ////    }
        ////}
        if (collision.transform.gameObject.TryGetComponent(out Destructible d))
        {
            //Debug.Log("콜리전 엔터 : " + collision.relativeVelocity.magnitude / Time.fixedDeltaTime);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        
    }
}
