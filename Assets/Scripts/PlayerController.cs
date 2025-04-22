using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private int _startingPipes = 25;


    [Header("References")]
    [SerializeField] private GameObject _waterPipePrefab;
    private GameObject _placementPreview;

    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;

    private Tilemap _tileMap;

    [Header("Data")]
    private int _currentPipes;

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

    private void Start()
    {
        _currentPipes = _startingPipes;

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

            CheckPlacement();

            if (_placementPreview.activeInHierarchy && Input.GetButton("Fire1"))
            {
                PlacePipe();
            }

            if (Input.GetButton("Fire2"))
            {
                RemovePipe();
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

    private void CheckPlacement()
    {
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
            }
            else
            {
                _placementPreview.SetActive(false);
            }
        }
    }

    private void PlacePipe()
    {
        if (_currentPipes > 0)
        {
            Instantiate(_waterPipePrefab, _tileMap.GetCellCenterLocal(_gridPosition), Quaternion.identity);
            _currentPipes--;
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
