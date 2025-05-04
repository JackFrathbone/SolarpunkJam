using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] string _characterName = "default";
    [SerializeField] Sprite _characterSprite;

    [SerializeField] DialogueObject _defaultDialogue;

    [Header("References")]
    private SpriteRenderer _spriteRenderer;

    [Header("Data")]
    private DialogueObject _currentDialogue;
    private List<DialogueObject> _runDialogues = new();

    private void OnValidate()
    {
        gameObject.name = "Character-" + _characterName;

        if (_characterSprite != null)
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _spriteRenderer.sprite = _characterSprite;
        }
    }

    private void Start()
    {
        _currentDialogue = _defaultDialogue;

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

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
        _currentDialogue = dialogueObject;
    }

    public DialogueObject GetCurrentDialogue()
    {
        if (!_runDialogues.Contains(_currentDialogue))
        {
            _runDialogues.Add(_currentDialogue);
        }

        return _currentDialogue;
    }

    public List<DialogueObject> GetRunDialogues()
    {
        return _runDialogues;
    }
}
