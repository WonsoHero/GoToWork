using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandControllerCanvas : MonoBehaviour
{
    //수치 확인용 텍스트
    [SerializeField] TextMeshProUGUI multiflierTmp;
    [SerializeField] TextMeshProUGUI maxHandSpeedTmp;
    [SerializeField] TextMeshProUGUI testTmp;

    [SerializeField] HandController handController;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ShowInfoText();
        UpdateMultifly();
    }
    void ShowInfoText()
    {
        if (multiflierTmp != null)
        {
            multiflierTmp.text = handController.multiflier.ToString();
        }
        if (maxHandSpeedTmp != null)
        {
            maxHandSpeedTmp.text = handController.maxHandSpeed.ToString();
        }
        if (testTmp != null)
        {
            //testTmp.text = leftHand.transform.rotation.eulerAngles.ToString();
        }
    }
    /*감도조절 함수*/
    void UpdateMultifly()
    {
        if (SceneManager.GetActiveScene().name == "SM_Hwang")
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                handController.multiflier -= 10;
                if (handController.multiflier < 0)
                {
                    handController.multiflier = 0;
                }
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                handController.multiflier += 10;
            }
            //maxSpeed Control
            if (Input.GetKeyDown(KeyCode.Q))
            {
                handController.maxHandSpeed -= 25;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                handController.maxHandSpeed += 25;
            }
        }
    }
    public void ShowText(string text)
    {
        testTmp.text = text;
    }
}
