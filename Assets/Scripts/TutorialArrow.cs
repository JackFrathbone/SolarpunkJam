using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialArrow : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] DialogueObject _tutorialDialogue;

    public DialogueObject GetTutorialDialogue()
    {
        return _tutorialDialogue;
    }
}
