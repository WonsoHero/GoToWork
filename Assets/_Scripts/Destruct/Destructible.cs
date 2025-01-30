using System;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] GameObject destructModel;

    public Action<bool> destruct;

    public float forceMultiplier = 4;
    public float destructForce = 4;

    GrabbaleObject grabbaleObject;
    bool destructed = false;
    float handForceFactor = 1;
    float bodyForceFactor = 10;
    float grabForce = 10f;

    private void Awake()
    {
        grabbaleObject = GetComponent<GrabbaleObject>();
    }

    private void OnCollisionStay(Collision collision)
    {
        //플레이 모드가 기본조작모드일때
        if (MissionManager.Instance.playerManager.State == PlayerState.Normal)
        {
            //몸통으로 뿌수고
            if(collision.gameObject.tag == "Body")
            {
                Debug.Log("몸통 충돌");
                DetectDestruct(collision, bodyForceFactor);
            }
        }
        //플레이 모드가 미션 조작모드일때
        else if(MissionManager.Instance.playerManager.State == PlayerState.Interaction)
        {
            //손으로 뿌순다
            //잡을 수 없는 오브젝트라면 양 손과의 충돌을 감지하고
            if(grabbaleObject == null)
            {
                if (collision.gameObject.tag == "LeftHand" || collision.gameObject.tag == "RightHand")
                {
                    Debug.Log("손 충돌");
                    DetectDestruct(collision, handForceFactor);
                }
            }
            //잡을수 있는 오브젝트라면
            else
            {
                //이 오브젝트를 잡은 손과의 충돌은 무시한다
                if (grabbaleObject.IsRightGrapped)
                {
                    if (collision.gameObject.tag == "LeftHand")
                    {
                        Debug.Log("손 충돌");
                        DetectDestruct(collision, handForceFactor);
                    }
                }
                else if(grabbaleObject.IsLeftGrapped)
                {
                    if (collision.gameObject.tag == "RightHand")
                    {
                        Debug.Log("손 충돌");
                        DetectDestruct(collision, handForceFactor);
                    }
                }
            }
        }
    }

    void DetectDestruct(Collision collision, float forceFactor)
    {
        //상대속도를 기준으로 충돌시 힘 계산
        Vector3 speed = collision.relativeVelocity;
        //Debug.Log("speed : " + speed);
        //F = 충격량 / 시간
        //collision.impulse로 충격량을 구하려 했으나 항상 (0,0,0) 나와서 못씀
        //질량을 1로 가정, 생략하고 속도의 크기를 충격량으로 삼음
        float force = speed.magnitude / Time.fixedDeltaTime;
        force *= forceFactor; //몸뚱이로 밀면 쉽게 부술 수 있게함
        //Debug.Log("forece : " + force);
        //충돌시 힘 크기가 destructForce를 초과하면 파괴
        //파괴 모델 하나만 나오도록 조건 추가
        if (force > destructForce && !destructed)
        {
            Destruct(collision.GetContact(0), speed);
        }
    }

    //손이 충돌하는 파괴
    public void Destruct(ContactPoint contact, Vector3 force)
    {
        destructed = true;
        destruct?.Invoke(destructed);

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

    //손으로 쥐어짜는 파괴
    public void Destruct()
    {
        destructed = true;
        destruct?.Invoke(destructed);

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
            cell.AddExplosionForce(grabForce, transform.position, 10f);
        }
    }
}
