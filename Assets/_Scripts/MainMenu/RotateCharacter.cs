using UnityEngine;

public class RotateCharacter : MonoBehaviour
{
    [SerializeField] float rotateY = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotateY * Time.deltaTime, 0);
    }
}
