using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager dialogueManager;
    public static DialogueManager Instance
    {
        get { return dialogueManager; }
    }

    [SerializeField] TextMeshProUGUI dialogueTmp;
    [SerializeField] GameObject dialoguePanel;
    public event Action<string> OnDialogueStart;

    private Dialogue currentDialogue;
    private int dialogueIndex;
    private bool isDialogueActive;
    private void Awake()
    {
        if (dialogueManager == null) dialogueManager = this;
        else Destroy(gameObject);
    }
    public void StartDialogue(Dialogue data)
    {
        if (isDialogueActive) return;

        currentDialogue = data;
        dialogueIndex = 0;
        isDialogueActive = true;
        dialoguePanel.SetActive(true);

        StartCoroutine(ShowDialogue());
    }
    IEnumerator ShowDialogue()
    {
        while(dialogueIndex<currentDialogue.dialogues.Length)
        {
            OnDialogueStart?.Invoke(currentDialogue.dialogues[dialogueIndex]);
            dialogueTmp.text=currentDialogue.dialogues[dialogueIndex];

            yield return new WaitUntil(()=>Input.GetKeyDown(KeyCode.Space));
            yield return new WaitUntil(()=>Input.GetKeyUp(KeyCode.Space));
            dialogueIndex++;
        }
        dialoguePanel.SetActive(false);
        isDialogueActive=false;
    }
}
