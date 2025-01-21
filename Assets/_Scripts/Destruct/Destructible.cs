using System;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] GameObject destructModel;

    public Action<bool> destruct;

    public float forceMultiplier = 4;
    public float destructForce = 4;

    bool destructed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("-----------------------\n" + gameObject.GetComponent<Rigidbody>().GetPointVelocity(transform.position));
    }

    private void OnCollisionEnter(Collision collision)
    {

        //Debug.Log("콜리전 엔터 : " + collision.relativeVelocity.magnitude / Time.fixedDeltaTime);

        
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "LeftHand" || collision.gameObject.tag == "RightHand")
        {
            //상대속도를 기준으로 충돌시 힘 계산
            Vector3 speed = collision.relativeVelocity;
            Debug.Log("speed : "+speed);
            //F = 충격량 / 시간
            //collision.impulse로 충격량을 구하려 했으나 항상 (0,0,0) 나와서 못씀
            //질량을 1로 가정, 생략하고 속도의 크기를 충격량으로 삼음
            float force = speed.magnitude / Time.fixedDeltaTime;
            Debug.Log("forece : " + force);
            //충돌시 힘 크기가 destructForce를 초과하면 파괴
            //파괴 모델 하나만 나오도록 조건 추가
            if (force > destructForce && !destructed)
            {
                Debug.Log("부서짐");
                destructed = true;
                destruct?.Invoke(destructed);
                Destruct(collision.GetContact(0), speed);

            }
        }
    }

    public void Destruct(ContactPoint contact, Vector3 force)
    {
        gameObject.SetActive(false);

        //화면 흔들림 및 소리도 여기서 처리

        //파괴 모델로 교체
        GameObject go = Instantiate(destructModel);
        go.transform.position = transform.position;
        go.transform.rotation = transform.rotation;

        //파괴 모델의 조각들에 힘을 가해 충돌방향으로 날라가게함
        Rigidbody[] cellRigidbodies = go.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody cell in cellRigidbodies)
        {
            cell.AddForceAtPosition(force * forceMultiplier ,contact.point, ForceMode.Impulse);
        }
    }
}
