using UnityEngine;

public class PowerSource : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _startActive;

    [Header("Data")]
    private bool _isActive;

    private void Start()
    {
        _isActive = _startActive;
    }

    public bool GetIsActive()
    {
        return _isActive;
    }
}
