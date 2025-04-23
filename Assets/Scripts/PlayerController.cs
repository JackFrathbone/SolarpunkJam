using RenderHeads.Services;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public enum PlayerMouseState
    {
        none,
        placement,
        powersource
    }


    [Header("Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private int _startingPipes = 25;
    [SerializeField] private int _startingParts = 5;

    [Header("Inventory")]
    private int _currentPipes;
    private int _currentParts;


    [Header("References")]
    [SerializeField] private GameObject _waterPipePrefab;
    private GameObject _placementPreview;

    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;

    private Tilemap _tileMap;

    private LazyService<WorldWaterManager> _worldWaterManager;

    [Header("Data")]
    private PlayerMouseState _mouseState = PlayerMouseState.none;

    private bool _facingLeft = false;
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

    private void Start()
    {
        _currentPipes = _startingPipes;
        _currentParts = _startingParts;

        _placementPreview = transform.Find("PlacePreview").gameObject;
        _placementPreview.SetActive(false);

        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _tileMap = FindObjectOfType<Tilemap>();
    }

    private void Update()
    {
        if (!_freezeInput)
        {
            _horizontalInput = Input.GetAxis("Horizontal");
            _verticalInput = Input.GetAxis("Vertical");

            _mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            CheckMouseState();

            switch (_mouseState)
            {
                case PlayerMouseState.none:
                    if (Input.GetButton("Fire2"))
                    {
                        RemovePipe();
                    }
                    break;
                case PlayerMouseState.placement:
                    if (Input.GetButton("Fire1") && _placementPreview.activeInHierarchy)
                    {
                        PlacePipe();
                    }
                    break;
                case PlayerMouseState.powersource:
                    if (Input.GetButtonDown("Fire1") && _targetPowerSource != null && _currentParts > 0)
                    {
                        if (_targetPowerSource.AddParts())
                        {
                            _currentParts--;
                        }
                    }
                    else if (Input.GetButtonDown("Fire2") && _targetPowerSource != null)
                    {
                        if (_targetPowerSource.RemoveParts())
                        {
                            _currentParts++;
                        }
                    }
                    break;
            }
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

            Destroy(pickup.gameObject);
        }
        else if (collision.CompareTag("PartsPickup"))
        {
            PartsPickup pickup = collision.GetComponent<PartsPickup>();

            _currentParts += pickup.partsToAdd;

            Destroy(pickup.gameObject);
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
            _spriteRenderer.flipX = false;
        }
        else if (_mouseWorldPosition.x < transform.position.x)
        {
            _spriteRenderer.flipX = true;
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
                if (_hit.collider.gameObject.CompareTag("PowerSource"))
                {
                    _targetPowerSource = _hit.collider.gameObject.GetComponent<PowerSource>();
                    _mouseState = PlayerMouseState.powersource;
                }
                else
                {
                    _mouseState = PlayerMouseState.none;
                }

                _placementPreview.SetActive(false);
            }
        }
    }

    private void PlacePipe()
    {
        if (_currentPipes > 0)
        {
            GameObject newPipe = Instantiate(_waterPipePrefab, _tileMap.GetCellCenterLocal(_gridPosition), Quaternion.identity, _worldWaterManager.Value.transform);
            _currentPipes--;
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
            }
        }
    }
}
