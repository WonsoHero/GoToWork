using UnityEngine;

public class HallwayDoor : MonoBehaviour
{
    [SerializeField] GameObject Trigger;
    [SerializeField] GameObject lightDoor;
    [SerializeField] GameObject clearArea;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenDoor()
    {
        Debug.Log("클리어");
        Trigger.SetActive(false);
        transform.Rotate(0, 220, 0);
        lightDoor.SetActive(true);
        clearArea.SetActive(true);
    }
}
