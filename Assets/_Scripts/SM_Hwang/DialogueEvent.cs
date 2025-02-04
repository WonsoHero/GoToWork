using UnityEngine;

public class DialogueEvent : MonoBehaviour
{
    [SerializeField] Dialogue dialogueData;

    private void OnEnable()
    {
        DialogueManager.Instance.StartDialogue(dialogueData);
    }
}