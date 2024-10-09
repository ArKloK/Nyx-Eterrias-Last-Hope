using UnityEngine;

public class DialogueBoxActivator : MonoBehaviour
{
    [SerializeField] GameObject dialogueBox;

    // Start is called before the first frame update
    void Start()
    {
        DialogueTrigger.OnTriggerDialogue += TriggerDialogue;
    }

    void OnDestroy()
    {
        DialogueTrigger.OnTriggerDialogue -= TriggerDialogue;
    }

    public void TriggerDialogue(Dialogue dialogue)
    {
        PauseMenuController.CanPause = false;
        if (dialogueBox.activeSelf)
        {
            StartCoroutine(DialogueManager.Instance.EnqueueDialogueFromInvoke(dialogue));
        }
        else
        {
            dialogueBox.SetActive(true);
            StartCoroutine(DialogueManager.Instance.StartDialogue(dialogue));
        }
    }
}
