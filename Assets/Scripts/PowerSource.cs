using TMPro;
using UnityEngine;

public class PowerSource : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _startActive;
    [SerializeField] private int _totalParts = 2;

    [Header("References")]
    private TextMeshPro _textMeshPro;

    [Header("Data")]
    private bool _isActive;

    private int _currentParts;

    private void Awake()
    {
        _textMeshPro = GetComponentInChildren<TextMeshPro>();
    }

    private void Start()
    {
        if (_startActive)
        {
            _isActive = _startActive;
            _currentParts = _totalParts;
        }

        CheckActive();
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
        }
        else
        {
            _textMeshPro.gameObject.SetActive(true);
            _isActive = false;
        }

        _textMeshPro.text = _currentParts + "/" + _totalParts;
    }
}
