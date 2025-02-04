using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public enum TriggerType { OnEnable, OnDestroy, OnTriggerEnter }
    [SerializeField] private TriggerType triggerCondition;
    [SerializeField] private Dialogue dialogueData;
    [SerializeField] private string colliderTagName = "Player";

    private void OnEnable()
    {
        if (triggerCondition == TriggerType.OnEnable)
        {
            StartDialogue();
        }
    }

    private void OnDestroy()
    {
        if (triggerCondition == TriggerType.OnDestroy)
        {
            StartDialogue();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerCondition == TriggerType.OnTriggerEnter && other.CompareTag(colliderTagName))
        {
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogueData);
    }
}
