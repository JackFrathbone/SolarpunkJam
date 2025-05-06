using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] string _characterName = "default";
    [SerializeField] Sprite _characterSprite;

    [SerializeField] DialogueObject _defaultDialogue;

    [Header("References")]
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] private GameObject _arrow;

    [Header("Data")]
    private DialogueObject _currentDialogue;
    private List<DialogueObject> _runDialogues = new();

    private void OnValidate()
    {
        gameObject.name = "Character-" + _characterName;

        _spriteRenderer.sprite = _characterSprite;
    }

    private void Start()
    {
        _currentDialogue = _defaultDialogue;

        if (_currentDialogue != null)
        {
            _arrow.SetActive(true);
        }
        else
        {
            _arrow.SetActive(false);
        }

        if (_characterSprite != null)
        {
            _spriteRenderer.sprite = _characterSprite;
        }
    }

    public string GetCharacterName()
    {
        return _characterName;
    }

    public Sprite GetCharacterSprite()
    {
        return _characterSprite;
    }

    public void SetNewDialogue(DialogueObject dialogueObject)
    {
        _arrow.SetActive(true);

        _currentDialogue = dialogueObject;
    }

    public DialogueObject GetCurrentDialogue()
    {
        if (!_runDialogues.Contains(_currentDialogue))
        {
            _runDialogues.Add(_currentDialogue);
        }

        DialogueObject dialogueObject = _currentDialogue;

        _arrow.SetActive(false);

        _currentDialogue = null;

        return dialogueObject;
    }

    public List<DialogueObject> GetRunDialogues()
    {
        return _runDialogues;
    }
}
