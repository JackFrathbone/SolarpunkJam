using RenderHeads.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoService
{
    [Header("References")]
    LazyService<GameManager> _gameManager;

    [SerializeField] TextMeshProUGUI _waterPipeCounter;
    [SerializeField] TextMeshProUGUI _partsCounter;
    [SerializeField] TextMeshProUGUI _cableCounter;

    //Dialogue
    [SerializeField] private GameObject _dialogue;
    [SerializeField] private TextMeshProUGUI _characterNameLabel;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Image _playerPotrait;
    [SerializeField] private Image _characterPotrait;

    [Header("Data")]
    private DialogueObject _currentDialogueObject;
    private int _currentDialogueIndex;

    private void Start()
    {
        _dialogue.SetActive(false);
        _nextButton.onClick.AddListener(delegate { NextDialogue(); }); ;
    }

    public void SetWaterPipeCounter(int i)
    {
        _waterPipeCounter.text = i.ToString();
    }

    public void SetPartsCounter(int i)
    {
        _partsCounter.text = i.ToString();
    }

    public void SetCableCounter(int i)
    {
        _cableCounter.text = i.ToString();
    }

    private bool SetDialogueText()
    {
        //If ran out of dialogue then return true
        if (_currentDialogueIndex >= _currentDialogueObject.dialogues.Count)
        {
            return true;
        }



        Dialogue dialogue = _currentDialogueObject.dialogues[_currentDialogueIndex];

        if (dialogue.isPlayer)
        {
            _characterPotrait.transform.localScale = Vector3.one;
            _playerPotrait.transform.localScale += Vector3.one * 0.5f;
        }
        else
        {
            _characterPotrait.transform.localScale += Vector3.one * 0.5f;
            _playerPotrait.transform.localScale = Vector3.one;
        }

        _dialogueText.text = dialogue.dialogueText;

        return false;
    }

    public void ActivateDialogue(DialogueObject dialogueObject, string characterName, Sprite characterSprite, Sprite playerSprite)
    {
        if (dialogueObject == null)
        {
            return;
        }

        _dialogue.SetActive(true);
        _characterNameLabel.text = characterName;

        _currentDialogueObject = dialogueObject;
        _currentDialogueIndex = 0;

        _characterPotrait.transform.localScale = Vector3.one;
        _playerPotrait.transform.localScale = Vector3.one;

        _characterPotrait.sprite = characterSprite;
        _playerPotrait.sprite = playerSprite;

        SetDialogueText();

        _gameManager.Value.PauseGame();
    }

    public void NextDialogue()
    {
        _currentDialogueIndex++;
        if (SetDialogueText())
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        _currentDialogueObject = null;
        _currentDialogueIndex = 0;

        _dialogue.SetActive(false);

        _gameManager.Value.UnPauseGame();
    }

}
