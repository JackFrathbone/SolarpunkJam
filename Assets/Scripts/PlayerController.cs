using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _moveSpeed = 5f;

    [Header("References")]
    [SerializeField] GameObject _waterPipePrefab;

    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;

    [Header("Data")]
    private bool _facingLeft = false;
    private bool _freezeMovement = false;

    //For movement
    private float _horizontalInput;
    private float _verticalInput;
    private Vector2 _inputVector;
    private Vector2 _newPosition;
    private Vector2 _mouseWorldPosition;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        if (!_freezeMovement)
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
        _mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

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
}
