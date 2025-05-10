using RenderHeads.Services;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public enum PlayerMouseState
    {
        none,
        placement,
        powersource,
        character
    }

    [Header("Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private int _startingPipes = 25;
    [SerializeField] private int _startingCables = 10;
    [SerializeField] private int _startingParts = 5;

    [SerializeField] private Sprite _dialoguePotraitSprite;

    [Header("Inventory")]
    private int _currentCables;
    private int _currentPipes;
    private int _currentParts;


    [Header("References")]
    [SerializeField] private GameObject _waterPipePrefab;
    [SerializeField] private GameObject _cablePrefab;

    [SerializeField] private GameObject _pickupLabel;

    private GameObject _placementPreview;

    private Rigidbody2D _rigidbody;

    private Tilemap _tileMap;

    private ParticleSystem _footParticles;

    private Animator _animator;

    private LazyService<GameManager> _gameManager;
    private LazyService<WorldWaterManager> _worldWaterManager;
    private LazyService<UIManager> _uiManager;

    [Header("Data")]
    private bool _placingCables;

    private PlayerMouseState _mouseState = PlayerMouseState.none;

    private bool _freezeInput = false;

    //For movement
    private float _horizontalInput;
    private float _verticalInput;
    private Vector2 _inputVector;
    private Vector2 _newPosition;
    private Vector2 _mouseWorldPosition;

    //For pipe placement
    private Vector3Int _gridPosition;
    private TileBase _hoveredTile;
    private Vector2 _cellCenterWorldPosition;
    private RaycastHit2D _hit;

    //For Powersource
    private PowerSource _targetPowerSource;

    private Character _targetCharacterController;

    private void Start()
    {
        _pickupLabel.SetActive(false);

        _gameManager.Value.playerController = this;

        _currentPipes = _startingPipes;
        _currentCables = _startingCables;
        _currentParts = _startingParts;

        UpdateUI();

        _placementPreview = transform.Find("PlacePreview").gameObject;
        _placementPreview.SetActive(false);

        _rigidbody = GetComponent<Rigidbody2D>();
        _footParticles = GetComponentInChildren<ParticleSystem>();
        _footParticles.Stop();

        _animator = GetComponentInChildren<Animator>();

        _tileMap = FindObjectOfType<Tilemap>();
    }

    private void Update()
    {
        if (!_freezeInput)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SwitchMode();
                _uiManager.Value.SetPlacementModeVisuals(_placingCables);
            }

            _horizontalInput = Input.GetAxis("Horizontal");
            _verticalInput = Input.GetAxis("Vertical");

            if (_horizontalInput != 0 || _verticalInput != 0)
            {
                _animator.SetBool("IsWalking", true);
                _footParticles.Play();
            }
            else
            {
                _animator.SetBool("IsWalking", false);
                _footParticles.Stop();
            }

            _mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            CheckMouseState();

            switch (_mouseState)
            {
                case PlayerMouseState.none:
                    if (Input.GetButton("Fire2"))
                    {
                        if (!_placingCables)
                        {
                            RemovePipe();
                        }
                        else
                        {
                            RemoveCable();
                        }
                    }
                    break;
                case PlayerMouseState.placement:
                    if (Input.GetButton("Fire1") && _placementPreview.activeInHierarchy)
                    {
                        if (!_placingCables)
                        {
                            PlacePipe();
                        }
                        else
                        {
                            PlaceCable();
                        }
                    }
                    break;
                case PlayerMouseState.powersource:
                    if (Input.GetButtonDown("Fire1") && _targetPowerSource != null && _currentParts > 0)
                    {
                        if (_targetPowerSource.AddParts())
                        {
                            _currentParts--;
                            UpdateUI();
                        }
                    }
                    else if (Input.GetButtonDown("Fire2") && _targetPowerSource != null)
                    {
                        if (_targetPowerSource.RemoveParts())
                        {
                            _currentParts++;
                            UpdateUI();
                        }
                    }
                    break;
                case PlayerMouseState.character:
                    if (Input.GetButtonDown("Fire1") && _targetCharacterController != null)
                    {
                        _uiManager.Value.ActivateDialogue(_targetCharacterController.GetCurrentDialogue(), _targetCharacterController.GetCharacterName(), _targetCharacterController.GetCharacterSprite(), _dialoguePotraitSprite);
                    }
                    break;
            }
        }
        else
        {
            _animator.SetBool("IsWalking", false);
        }
    }

    private void FixedUpdate()
    {
        if (!_freezeInput)
        {
            CheckMovement();
            CheckLookDirection();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("WaterPipePickup"))
        {
            WaterPipePickup pickup = collision.GetComponent<WaterPipePickup>();

            _currentPipes += pickup.pipesToAdd;
            StartCoroutine(ShowPickupNumber(pickup.pipesToAdd));
            UpdateUI();

            Destroy(pickup.gameObject);
        }
        else if (collision.CompareTag("PartsPickup"))
        {
            PartsPickup pickup = collision.GetComponent<PartsPickup>();

            _currentParts += pickup.partsToAdd;
            StartCoroutine(ShowPickupNumber(pickup.partsToAdd));
            UpdateUI();

            Destroy(pickup.gameObject);
        }
        else if (collision.CompareTag("CablePickup"))
        {
            CablePickup pickup = collision.GetComponent<CablePickup>();

            _currentCables += pickup.cableToAdd;
            StartCoroutine(ShowPickupNumber(pickup.cableToAdd));
            UpdateUI();

            Destroy(pickup.gameObject);
        }
        else if (collision.CompareTag("Tutorial"))
        {
            TutorialArrow tutorial = collision.GetComponent<TutorialArrow>();

            _uiManager.Value.ActivateDialogue(tutorial.GetTutorialDialogue(), "", null, _dialoguePotraitSprite);
            Destroy(collision.gameObject);
        }
    }

    private void CheckMovement()
    {
        _inputVector = new Vector2(_horizontalInput, _verticalInput);
        _inputVector = Vector2.ClampMagnitude(_inputVector, 1);
        _newPosition = _rigidbody.position + (_inputVector * _moveSpeed) * Time.deltaTime;

        _rigidbody.MovePosition(_newPosition);
    }

    private void CheckLookDirection()
    {
        if (_mouseWorldPosition.x > transform.position.x)
        {
            _animator.transform.localScale = new Vector3(-0.4f, _animator.transform.localScale.y, _animator.transform.localScale.z);
        }
        else if (_mouseWorldPosition.x < transform.position.x)
        {
            _animator.transform.localScale = new Vector3(0.4f, _animator.transform.localScale.y, _animator.transform.localScale.z);
        }
        else
        {
            return;
        }
    }

    private void CheckMouseState()
    {
        //Stuff to clear
        _targetPowerSource = null;
        //

        _gridPosition = _tileMap.WorldToCell(_mouseWorldPosition);
        _hoveredTile = _tileMap.GetTile(_gridPosition);
        _cellCenterWorldPosition = _tileMap.GetCellCenterWorld(_gridPosition);

        if (_hoveredTile == null)
        {
            _placementPreview.SetActive(false);
            return;
        }
        else
        {
            _hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (_hit.collider == null)
            {
                _placementPreview.SetActive(true);
                _placementPreview.transform.position = _cellCenterWorldPosition;

                _mouseState = PlayerMouseState.placement;
            }
            else
            {
                _placementPreview.SetActive(false);

                if (_hit.collider.gameObject.CompareTag("PowerSource"))
                {
                    _targetPowerSource = _hit.collider.gameObject.GetComponent<PowerSource>();
                    _mouseState = PlayerMouseState.powersource;
                }
                else if (_hit.collider.gameObject.CompareTag("Character"))
                {
                    if (_hit.collider.isTrigger)
                    {
                        _targetCharacterController = _hit.collider.gameObject.GetComponent<Character>();
                    }
                    else
                    {
                        _targetCharacterController = _hit.collider.gameObject.GetComponentInParent<Character>();
                    }

                    _mouseState = PlayerMouseState.character;
                }
                else
                {
                    _mouseState = PlayerMouseState.none;
                }
            }
        }
    }

    private void PlacePipe()
    {
        if (_currentPipes > 0)
        {
            GameObject newPipe = Instantiate(_waterPipePrefab, _tileMap.GetCellCenterLocal(_gridPosition), Quaternion.identity, _worldWaterManager.Value.transform);
            _currentPipes--;
            UpdateUI();
            newPipe.name = "Pipe" + _currentPipes.ToString();
        }
    }

    private void RemovePipe()
    {
        _hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (_hit.collider != null)
        {
            if (_hit.collider.CompareTag("Pipe"))
            {
                Destroy(_hit.collider.gameObject);
                _currentPipes++;
                UpdateUI();
            }
        }
    }

    private void PlaceCable()
    {
        if (_currentCables > 0)
        {
            GameObject newCable = Instantiate(_cablePrefab, _tileMap.GetCellCenterLocal(_gridPosition), Quaternion.identity, _worldWaterManager.Value.transform);
            _currentCables--;
            UpdateUI();
            newCable.name = "Cable" + _currentCables.ToString();
        }
    }

    private void RemoveCable()
    {
        _hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (_hit.collider != null)
        {
            if (_hit.collider.CompareTag("Cable"))
            {
                Destroy(_hit.collider.gameObject);
                _currentCables++;
                UpdateUI();
            }
        }
    }

    private void UpdateUI()
    {
        _uiManager.Value.SetWaterPipeCounter(_currentPipes);
        _uiManager.Value.SetPartsCounter(_currentParts);
        _uiManager.Value.SetCableCounter(_currentCables);
    }

    public void SetFreezeInput(bool freezeInput)
    {
        _freezeInput = freezeInput;
    }

    public bool SwitchMode()
    {
        return _placingCables = !_placingCables;
    }

    IEnumerator ShowPickupNumber(int number)
    {
        _pickupLabel.SetActive(true);
        _pickupLabel.GetComponentInChildren<TextMeshPro>().text = "+" + number.ToString() + "!";
        yield return new WaitForSeconds(3f);
        _pickupLabel.SetActive(false);
    }
}
