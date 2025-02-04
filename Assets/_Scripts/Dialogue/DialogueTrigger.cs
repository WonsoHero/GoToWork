using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public enum TriggerType { OnEnable, OnDisable, OnDestroy, OnTriggerEnter }
    public enum DialogueRandom { True, False }
    [SerializeField] private TriggerType triggerCondition;
    [SerializeField] private DialogueRandom randomCondition = DialogueRandom.False;
    [SerializeField] private Dialogue dialogueData;
    [SerializeField] private string colliderTagName = "Player";

    private void OnEnable()
    {
        if (triggerCondition == TriggerType.OnEnable)
        {
            StartDialogue();
        }
    }
    private void OnDisable()
    {
        if (triggerCondition == TriggerType.OnDisable)
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
            Destroy(gameObject);
        }
    }
    private void StartDialogue()
    {
        if (randomCondition == DialogueRandom.False)
        {
            DialogueManager.Instance.StartDialogue(dialogueData);
        }
        else if (randomCondition == DialogueRandom.True)
        {
            DialogueManager.Instance.RandomDialogue(dialogueData);
        }
    }
}