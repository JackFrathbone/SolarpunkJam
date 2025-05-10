using RenderHeads.Services;
using System.Collections.Generic;
using UnityEngine;

public class WaterSourcePump : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<PowerSource> _connectedPowerSources = new();

    [SerializeField] private GameObject _activeIcon;

    [SerializeField] private GameObject _ArrowUpRight;
    [SerializeField] private GameObject _ArrowDownLeft;
    [SerializeField] private GameObject _ArrowUpLeft;
    [SerializeField] private GameObject _ArrowDownRight;

    LazyService<WorldWaterManager> _worldWaterManager;

    [Header("Data")]
    private RaycastHit2D _hit;

    //For keeping track of directions to raycast
    private readonly Vector2 _isometricUpRight = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 30f), Mathf.Sin(Mathf.Deg2Rad * 30f)).normalized;
    private readonly Vector2 _isometricDownLeft = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 210f), Mathf.Sin(Mathf.Deg2Rad * 210f)).normalized;
    private readonly Vector2 _isometricUpLeft = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 150f), Mathf.Sin(Mathf.Deg2Rad * 150f)).normalized;
    private readonly Vector2 _isometricDownRight = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 330f), Mathf.Sin(Mathf.Deg2Rad * 330f)).normalized;

    private void Start()
    {
        _worldWaterManager.Value.AddWaterPump(this);

        _activeIcon.SetActive(false);
    }

    private void FixedUpdate()
    {
        CheckForConnections();
    }

    private void CheckForConnections()
    {
        gameObject.layer = 2;

        _hit = Physics2D.Raycast(transform.position, _isometricUpRight, 0.5f);
        CheckHit(0);

        _hit = Physics2D.Raycast(transform.position, _isometricDownLeft, 0.5f);
        CheckHit(1);

        _hit = Physics2D.Raycast(transform.position, _isometricUpLeft, 0.5f);
        CheckHit(2);

        _hit = Physics2D.Raycast(transform.position, _isometricDownRight, 0.5f);
        CheckHit(3);

        gameObject.layer = 0;
    }

    private void CheckHit(int direction)
    {
        if (_hit.collider == null)
        {
            SetDirectionArrow(direction, true);

            return;
        }

        if (_hit.collider.CompareTag("Cable"))
        {
            SetDirectionArrow(direction, false);
        }
        else
        {
            SetDirectionArrow(direction, false);
        }
    }



    private void UpdateActiveIcon()
    {
        if (_connectedPowerSources.Count == 0)
        {
            _activeIcon.SetActive(false);
        }
        else
        {
            bool hasPower = false;
            foreach (PowerSource powerSource in _connectedPowerSources)
            {
                if (powerSource.GetIsActive())
                {
                    hasPower = true;
                }
            }

            _activeIcon.SetActive(hasPower);
        }
    }

    private void SetDirectionArrow(int direction, bool setActive)
    {
        switch (direction)
        {
            case 0:
                _ArrowUpRight.SetActive(setActive);
                break;
            case 1:
                _ArrowDownLeft.SetActive(setActive);
                break;
            case 2:
                _ArrowUpLeft.SetActive(setActive);
                break;
            case 3:
                _ArrowDownRight.SetActive(setActive);
                break;
        }
    }

    public List<PowerSource> GetConnectedPowerSources()
    {
        return _connectedPowerSources;
    }

    public void ClearConnectedPowerSources()
    {
        _connectedPowerSources.Clear();
        UpdateActiveIcon();
    }

    public void AddConnectedPowerSource(PowerSource powerSource)
    {
        if (!_connectedPowerSources.Contains(powerSource))
        {
            _connectedPowerSources.Add(powerSource);
        }

        UpdateActiveIcon();
    }

    public void RemoveConnectedPowerSource(PowerSource powerSource)
    {
        if (_connectedPowerSources.Contains(powerSource))
        {
            _connectedPowerSources.Remove(powerSource);
        }

        UpdateActiveIcon();
    }
}
