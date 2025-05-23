using RenderHeads.Services;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoService
{
    [Header("References")]
    LazyService<GameManager> _gameManager;

    [SerializeField] TextMeshProUGUI _waterPipeCounter;
    [SerializeField] TextMeshProUGUI _partsCounter;
    [SerializeField] TextMeshProUGUI _cableCounter;

    [SerializeField] private string _playerName;

    [SerializeField] private GameObject _quitMenu;

    //Dialogue
    [SerializeField] private GameObject _dialogue;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Image _playerPotrait;
    [SerializeField] private Image _characterPotrait;
    [SerializeField] private GameObject _dialogueGO;
    [SerializeField] private Sprite _dialogueLeftImg;
    [SerializeField] private Sprite _dialogueRightImg;
    [SerializeField] private TextMeshProUGUI _speakerName;
    [SerializeField] private Transform _speakerNameLeft;
    [SerializeField] private Transform _speakerNameRight;


    [SerializeField] private Button _modeButton;
    [SerializeField] private Sprite _modePipesImg;
    [SerializeField] private Sprite _modeCablesImg;

    [SerializeField] private AudioClip _clickPopClip;
    [SerializeField] private AudioClip _switchBloopClip;

    [Header("Data")]
    private DialogueObject _currentDialogueObject;
    private int _currentDialogueIndex;

    private string _currentCharacterName;

    private void Start()
    {
        _dialogue.SetActive(false);
        _nextButton.onClick.AddListener(delegate { NextDialogue(); });

        _modeButton.onClick.AddListener(SwitchPlacementMode);
        _modeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Pipes";
        _modeButton.GetComponent<Image>().sprite = _modePipesImg;

        _quitMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            _quitMenu.SetActive(true);
        }
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void CancelQuit()
    {
        _quitMenu.SetActive(false);
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
            _characterPotrait.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            _playerPotrait.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
            _dialogueGO.GetComponent<Image>().sprite = _dialogueLeftImg;
            _speakerName.GetComponent<Transform>().position = _speakerNameLeft.position;
            _speakerName.text = _playerName;
        }
        else
        {
            _characterPotrait.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
            _playerPotrait.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            _dialogueGO.GetComponent<Image>().sprite = _dialogueRightImg;
            _speakerName.GetComponent<Transform>().position = _speakerNameRight.position;
            _speakerName.text = _currentCharacterName;
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
        _currentCharacterName = characterName;

        _currentDialogueObject = dialogueObject;
        _currentDialogueIndex = 0;

        _characterPotrait.transform.localScale = Vector3.one;
        _playerPotrait.transform.localScale = Vector3.one;


        _characterPotrait.sprite = characterSprite;
        _playerPotrait.sprite = playerSprite;

        if (_characterPotrait.sprite == null)
        {
            _characterPotrait.color = Color.clear;
        }
        else
        {
            _characterPotrait.color = Color.white;
        }

        SetDialogueText();

        _gameManager.Value.PauseGame();
    }

    public void NextDialogue()
    {
        _currentDialogueIndex++;

        _gameManager.Value.PlayAudioClip(_clickPopClip, 0.9f, Random.Range(1.25f, 1.5f));

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

    private void SwitchPlacementMode()
    {
        bool currentMode = _gameManager.Value.SwitchPlayerPlacementMode();

        SetPlacementModeVisuals(currentMode);
    }

    public void SetPlacementModeVisuals(bool currentMode)
    {
        _gameManager.Value.PlayAudioClip(_switchBloopClip, 1f, 1f);

        if (currentMode)
        {
            _modeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Cables";
            _modeButton.GetComponent<Image>().sprite = _modeCablesImg;
        }
        else
        {
            _modeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Pipes";
            _modeButton.GetComponent<Image>().sprite = _modePipesImg;
        }
    }
}
