using JetBrains.Annotations;
using RenderHeads.Services;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class WaterInput : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _requiredWaterAmount = 6;
    [SerializeField] private List<Blocker> _connectedBlockers = new();

    [SerializeField] private List<VisualSwitch> _connectedVisuals = new();

    [SerializeField] private UnityEvent _RunOnceEvent;
    [SerializeField] private UnityEvent _RunOnActivateEvent;

    [Header("References")]
    [SerializeField] private GameObject _activeIcon;

    [SerializeField] private GameObject _ArrowUpRight;
    [SerializeField] private GameObject _ArrowDownLeft;
    [SerializeField] private GameObject _ArrowUpLeft;
    [SerializeField] private GameObject _ArrowDownRight;

    private LazyService<WorldWaterManager> _worldWaterManager;

    [Header("Data")]
    private int _currentWaterAmount;
    private bool _hasWater;

    private bool _hasRunOnce;

    private TextMeshPro _textMeshPro;

    private List<WaterPipe> _connectedPipes = new();
    private List<WaterSource> _connectedWaterSources = new();

    private RaycastHit2D _hit;

    //For keeping track of directions to raycast
    private readonly Vector2 _isometricUpRight = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 30f), Mathf.Sin(Mathf.Deg2Rad * 30f)).normalized;
    private readonly Vector2 _isometricDownLeft = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 210f), Mathf.Sin(Mathf.Deg2Rad * 210f)).normalized;
    private readonly Vector2 _isometricUpLeft = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 150f), Mathf.Sin(Mathf.Deg2Rad * 150f)).normalized;
    private readonly Vector2 _isometricDownRight = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 330f), Mathf.Sin(Mathf.Deg2Rad * 330f)).normalized;

    private void Awake()
    {
        _textMeshPro = GetComponentInChildren<TextMeshPro>();
        _textMeshPro.text = _currentWaterAmount + "/" + _requiredWaterAmount;
    }

    private void Start()
    {
        _worldWaterManager.Value.AddWaterInput(this);

        Deactivate();

        _activeIcon.SetActive(false);

        foreach (VisualSwitch visual in _connectedVisuals)
        {
            visual.AddWaterInput(this);
        }
    }

    private void FixedUpdate()
    {
        CheckForConnections();
        CheckConnectedPipeWater();
    }

    private void CheckForConnections()
    {
        _connectedPipes.Clear();

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

        if (_hit.collider.CompareTag("Pipe"))
        {
            _connectedPipes.Add(_hit.collider.GetComponent<WaterPipe>());

            SetDirectionArrow(direction, false);
        }
        else
        {
            SetDirectionArrow(direction, false);
        }
    }

    private void CheckConnectedPipeWater()
    {
        bool _connectedPipeHasWater = false;

        foreach (WaterPipe pipe in _connectedPipes)
        {
            if (pipe.hasWater)
            {
                _connectedPipeHasWater = true;
                break;
            }
        }

        UpdateState(_connectedPipeHasWater);
    }

    private void UpdateState(bool hasWater)
    {
        _hasWater = CheckHasWater();

        if (_hasWater)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }

        _textMeshPro.text = _currentWaterAmount + "/" + _requiredWaterAmount;
    }

    private bool CheckHasWater()
    {
        _currentWaterAmount = 0;

        foreach (WaterSource waterSource in _connectedWaterSources)
        {
            //Make sure the water source is powered
            if (waterSource.GetIsPowered())
            {
                _currentWaterAmount += waterSource.GetWaterSourceAmount();
            }
        }

        if (_currentWaterAmount == _requiredWaterAmount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Activate()
    {
        _activeIcon.SetActive(true);

        foreach (Blocker blocker in _connectedBlockers)
        {
            blocker.OpenBlocker();
        }

        if (!_hasRunOnce)
        {
            _RunOnceEvent.Invoke();
            _hasRunOnce = true;
        }

        _RunOnActivateEvent.Invoke();
    }

    private void Deactivate()
    {
        _activeIcon.SetActive(false);

        foreach (Blocker blocker in _connectedBlockers)
        {
            blocker.CloseBlocker();
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

    public void AddConnectedWaterSource(WaterSource waterSource)
    {
        if (!_connectedWaterSources.Contains(waterSource))
        {
            _currentWaterAmount += waterSource.GetWaterSourceAmount();
            _connectedWaterSources.Add(waterSource);
        }
    }

    public void RemoveConnectedWaterSource(WaterSource waterSource)
    {
        if (_connectedWaterSources.Contains(waterSource))
        {
            _currentWaterAmount -= waterSource.GetWaterSourceAmount();
            _connectedWaterSources.Remove(waterSource);
        }
    }

    public void ClearConnectedWaterSources()
    {
        _currentWaterAmount = 0;
        _connectedWaterSources.Clear();
    }

    public bool GetHasWater()
    {
        return _hasWater;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, _isometricUpRight);
        Gizmos.DrawRay(transform.position, _isometricDownLeft);
        Gizmos.DrawRay(transform.position, _isometricUpLeft);
        Gizmos.DrawRay(transform.position, _isometricDownRight);


        //Draws lines to each connected water visual
        if (_connectedVisuals != null && _connectedVisuals.Count > 0)
        {
            Gizmos.color = Color.red;

            // Iterate through each GameObject in the list.
            foreach (VisualSwitch visual in _connectedVisuals)
            {
                // Make sure the target GameObject is not null.
                if (visual.gameObject != null)
                {
                    // Draw the line from the current GameObject's position to the target's position.
                    Gizmos.DrawLine(transform.position, visual.gameObject.transform.position);
                }
            }
        }

        //Draws lines to each connected water visual
        if (_connectedBlockers != null && _connectedBlockers.Count > 0)
        {
            Gizmos.color = Color.green;

            // Iterate through each GameObject in the list.
            foreach (Blocker blocker in _connectedBlockers)
            {
                // Make sure the target GameObject is not null.
                if (blocker.gameObject != null)
                {
                    // Draw the line from the current GameObject's position to the target's position.
                    Gizmos.DrawLine(transform.position, blocker.gameObject.transform.position);
                }
            }
        }
    }
}
