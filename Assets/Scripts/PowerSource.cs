using RenderHeads.Services;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerSource : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _startActive;
    [SerializeField] private int _totalParts = 6;

    [SerializeField] Sprite _activeSprite;
    [SerializeField] Sprite _inactiveSprite;

    [SerializeField] AudioClip _changePartsClip;

    [Header("References")]
    [SerializeField] private GameObject _activeIcon;

    private SpriteRenderer _spriteRenderer;
    private List<Cable> _connectedCables = new();
    private TextMeshPro _textMeshPro;

    private LazyService<GameManager> _gameManager;
    private LazyService<WorldWaterManager> _worldWaterManager;

    [Header("Data")]
    private bool _isActive;
    private int _currentParts;

    private RaycastHit2D _hit;

    //For keeping track of directions to raycast
    private readonly Vector2 _isometricUpRight = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 30f), Mathf.Sin(Mathf.Deg2Rad * 30f)).normalized;
    private readonly Vector2 _isometricDownLeft = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 210f), Mathf.Sin(Mathf.Deg2Rad * 210f)).normalized;
    private readonly Vector2 _isometricUpLeft = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 150f), Mathf.Sin(Mathf.Deg2Rad * 150f)).normalized;
    private readonly Vector2 _isometricDownRight = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 330f), Mathf.Sin(Mathf.Deg2Rad * 330f)).normalized;

    private void Awake()
    {
        _textMeshPro = GetComponentInChildren<TextMeshPro>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _worldWaterManager.Value.AddPowerSource(this);

        if (_startActive)
        {
            _spriteRenderer.sprite = _activeSprite;

            _isActive = _startActive;
            _currentParts = _totalParts;

            _activeIcon.SetActive(true);
        }
        else
        {
            _spriteRenderer.sprite = _inactiveSprite;

            _activeIcon.SetActive(false);
        }

        CheckActive();
    }

    private void FixedUpdate()
    {
        if (!GetIsActive())
        {
            return;
        }

        CheckForConnections();
    }

    private void CheckForConnections()
    {
        _connectedCables.Clear();

        gameObject.layer = 2;

        _hit = Physics2D.Raycast(transform.position, _isometricUpRight, 0.5f);
        CheckHit();

        _hit = Physics2D.Raycast(transform.position, _isometricDownLeft, 0.5f);
        CheckHit();

        _hit = Physics2D.Raycast(transform.position, _isometricUpLeft, 0.5f);
        CheckHit();

        _hit = Physics2D.Raycast(transform.position, _isometricDownRight, 0.5f);
        CheckHit();

        gameObject.layer = 0;
    }

    private void CheckHit()
    {
        if (_hit.collider == null)
        {
            return;
        }

        if (_hit.collider.CompareTag("Cable"))
        {
            _connectedCables.Add(_hit.collider.GetComponent<Cable>());
        }
    }

    public List<Cable> GetConnectedCables()
    {
        return _connectedCables;
    }

    public bool GetIsActive()
    {
        return _isActive;
    }

    public bool AddParts()
    {
        if (_currentParts < _totalParts)
        {
            _gameManager.Value.PlayAudioClip(_changePartsClip, 0.25f, 1f);

            _currentParts++;
            CheckActive();
            return true;
        }
        else
        {
            return false;
        }
    }


    public bool RemoveParts()
    {
        if (_currentParts > 0)
        {
            _gameManager.Value.PlayAudioClip(_changePartsClip, 0.25f, 0.85f);

            _currentParts--;
            CheckActive();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CheckActive()
    {
        if (_currentParts >= _totalParts)
        {
            _spriteRenderer.sprite = _activeSprite;

            _textMeshPro.transform.parent.gameObject.SetActive(false);
            _isActive = true;

            _activeIcon.SetActive(true);
        }
        else
        {
            _spriteRenderer.sprite = _inactiveSprite;

            _textMeshPro.transform.parent.gameObject.SetActive(true);
            _isActive = false;

            _activeIcon.SetActive(false);
        }

        _textMeshPro.text = _currentParts + "/" + _totalParts;
    }
}
