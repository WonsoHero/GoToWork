using UnityEngine;

public class TitleDestruct : MonoBehaviour
{
    [SerializeField] GameObject destructModel;
    [SerializeField] Vector3 forceDirection=new Vector3(-5,0,10);
    private void OnTriggerEnter(Collider other)
    {
        GameObject go = Instantiate(destructModel);
        go.transform.position = transform.position;
        go.transform.rotation = transform.rotation;

        //파괴 모델의 조각들에 힘을 가해 충돌방향으로 날라가게함
        Rigidbody[] cellRigidbodies = go.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody cell in cellRigidbodies)
        {
            cell.AddForceAtPosition(forceDirection, cell.position, ForceMode.Impulse);
        }
        gameObject.SetActive(false);
    }
}
