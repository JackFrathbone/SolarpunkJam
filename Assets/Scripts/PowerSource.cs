using RenderHeads.Services;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerSource : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _startActive;
    [SerializeField] private int _totalParts = 6;

    [Header("References")]
    [SerializeField] private GameObject _activeIcon;

    private List<Cable> _connectedCables = new();

    private TextMeshPro _textMeshPro;

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
    }

    private void Start()
    {
        _worldWaterManager.Value.AddPowerSource(this);

        if (_startActive)
        {
            _isActive = _startActive;
            _currentParts = _totalParts;

            _activeIcon.SetActive(true);
        }
        else
        {
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
            _textMeshPro.gameObject.SetActive(false);
            _isActive = true;

            _activeIcon.SetActive(true);
        }
        else
        {
            _textMeshPro.gameObject.SetActive(true);
            _isActive = false;

            _activeIcon.SetActive(false);
        }

        _textMeshPro.text = _currentParts + "/" + _totalParts;
    }
}
