using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialArrow : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] DialogueObject _tutorialDialogue;

    [SerializeField] private UnityEvent _RunOnDestroyEvent;

    public DialogueObject GetTutorialDialogue()
    {
        return _tutorialDialogue;
    }

    private void OnDestroy()
    {
        _RunOnDestroyEvent.Invoke();
    }
}
