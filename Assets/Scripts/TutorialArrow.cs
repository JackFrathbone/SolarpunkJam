using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialArrow : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] DialogueObject _tutorialDialogue;

    [SerializeField] private UnityEvent _RunOnDestroyEvent;

    [SerializeField] bool _startHidden;

    private void Start()
    {
        if (_startHidden)
        {
            gameObject.SetActive(false);
        }
    }

    public DialogueObject GetTutorialDialogue()
    {
        return _tutorialDialogue;
    }

    private void OnDestroy()
    {
        _RunOnDestroyEvent.Invoke();
    }
}
