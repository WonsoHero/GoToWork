using System;
using System.Text;
using TMPro;
using UnityEngine;

public class MissionContent : MonoBehaviour
{
    [SerializeField] TMP_StyleSheet strikeThrough;
    [SerializeField] TMP_Text missionText;

    public bool cleared = false;

    StringBuilder sb = new StringBuilder();

    private void Awake()
    {
        sb.Append("<s>");
    }

    public void MissionAdd(MissionOBJ missionOBJ)
    {
        missionOBJ.succeed += MissionClear;
    }
    public void MissionRemove(MissionOBJ missionOBJ)
    {
        missionOBJ.succeed -= MissionClear;
    }

    public void MissionClear(bool clear)
    {
        cleared = clear;

        if (cleared)
        {
            var color = missionText.color;
            color.a = 0.1f;
            missionText.color = color;

            //sb.Append(missionText.text);
            //sb.Append("</s>");

            //TextChange(sb.ToString());
        }
    }

    public void TextChange(string texts)
    {
        missionText.text = texts;
    }
}
