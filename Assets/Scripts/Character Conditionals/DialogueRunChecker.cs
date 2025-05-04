using UnityEngine;
using UnityEngine.Events;

public class DialogueRunChecker : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] DialogueObject _dialogueToCheck;
    [SerializeField] UnityEvent _RunOnDialogueCompleteEvent;

    [Header("Data")]
    private Character _character;

    private void Start()
    {
        _character = GetComponent<Character>();
    }

    private void Update()
    {
        if (_character.GetRunDialogues().Contains(_dialogueToCheck))
        {
            _RunOnDialogueCompleteEvent.Invoke();

            Destroy(this);
        }
    }
}
