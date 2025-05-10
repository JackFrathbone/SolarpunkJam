using RenderHeads.Services;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaterSource : MonoBehaviour
{
    [Header("Settings")]
    //If this is attached to a water input, will only active when the input is active
    [SerializeField] private WaterInput _parentWaterInput;
    [SerializeField, Tooltip("How much water this water source has")] private int _waterSourceAmount = 6;
    [SerializeField] private List<WaterSourcePump> _connectedPumps = new();

    [SerializeField] private Color _textBoxIncorrectAmountColour;

    [Header("References")]
    private List<WaterPipe> _connectedPipes = new();
    private TextMeshPro _textMeshPro;

    [SerializeField] GameObject _ActiveIcon;

    [SerializeField] private GameObject _ArrowUpRight;
    [SerializeField] private GameObject _ArrowDownLeft;
    [SerializeField] private GameObject _ArrowUpLeft;
    [SerializeField] private GameObject _ArrowDownRight;

    private SpriteRenderer _textBackgroundRenderer;

    private LazyService<WorldWaterManager> _worldWaterManager;

    [Header("Data")]
    private bool _isPowered;
    private RaycastHit2D _hit;

    //For keeping track of directions to raycast
    private readonly Vector2 _isometricUpRight = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 30f), Mathf.Sin(Mathf.Deg2Rad * 30f)).normalized;
    private readonly Vector2 _isometricDownLeft = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 210f), Mathf.Sin(Mathf.Deg2Rad * 210f)).normalized;
    private readonly Vector2 _isometricUpLeft = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 150f), Mathf.Sin(Mathf.Deg2Rad * 150f)).normalized;
    private readonly Vector2 _isometricDownRight = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 330f), Mathf.Sin(Mathf.Deg2Rad * 330f)).normalized;

    private void Awake()
    {
        _textMeshPro = GetComponentInChildren<TextMeshPro>();
        _textMeshPro.text = _waterSourceAmount.ToString();

        _textBackgroundRenderer = _textMeshPro.transform.parent.gameObject.GetComponent<SpriteRenderer>();

        if (_parentWaterInput != null)
        {
            _textMeshPro.transform.parent.gameObject.SetActive(false);
            _ActiveIcon.SetActive(false);
        }
    }

    private void Start()
    {
        _worldWaterManager.Value.AddWaterSource(this);
    }

    private void FixedUpdate()
    {
        /*       if (CheckParentWaterInput())
               {
                   return;
               }

               CheckPowered();

               if (!_isPowered)
               {
                   return;
               }*/


        CheckPowered();
        CheckForConnections();

        if (!_isPowered)
        {
            _ArrowUpRight.SetActive(false);
            _ArrowDownRight.SetActive(false);
            _ArrowUpLeft.SetActive(false);
            _ArrowDownLeft.SetActive(false);
        }
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
            SetDirectionArrow(direction, false);

            if (_isPowered)
            {
                _connectedPipes.Add(_hit.collider.GetComponent<WaterPipe>());
            }
        }
        else
        {
            SetDirectionArrow(direction, false);
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

    public bool CheckParentWaterInput()
    {
        if (_parentWaterInput == null)
        {
            return false;
        }

        if (_parentWaterInput.GetHasWater())
        {
            _ActiveIcon.SetActive(true);

            _textMeshPro.transform.parent.gameObject.SetActive(true);
            return false;
        }
        else
        {
            _ActiveIcon.SetActive(false);

            _textMeshPro.transform.parent.gameObject.SetActive(false);
            return true;
        }
    }

    public List<WaterPipe> GetConnectedPipes()
    {
        return _connectedPipes;
    }

    public bool GetIsPowered()
    {
        return _isPowered;
    }

    public int GetWaterSourceAmount()
    {
        return _waterSourceAmount;
    }

    private void CheckPowered()
    {
        if (_connectedPumps.Count == 0)
        {
            _isPowered = true;
            _textBackgroundRenderer.color = Color.white;
            return;
        }

        //Go through all connected pumps, check their connected power sources, if one isnt powered up then turn off this water source
        bool poweredUp = true;
        bool nullCheck = false;

        foreach (WaterSourcePump pump in _connectedPumps)
        {
            foreach (PowerSource powerSource in pump.GetConnectedPowerSources())
            {
                nullCheck = true;

                if (!powerSource.GetIsActive())
                {
                    poweredUp = false;
                }
            }
        }

        if (!nullCheck)
        {
            poweredUp = false;
        }

        _isPowered = poweredUp;

        if (_isPowered)
        {
            _textBackgroundRenderer.color = Color.white;
        }
        else
        {
            _textBackgroundRenderer.color = _textBoxIncorrectAmountColour;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        //Draw rays for each direction of detection
        Gizmos.DrawRay(transform.position, _isometricUpRight);
        Gizmos.DrawRay(transform.position, _isometricDownLeft);
        Gizmos.DrawRay(transform.position, _isometricUpLeft);
        Gizmos.DrawRay(transform.position, _isometricDownRight);

        //Draws lines to each connected pump
        if (_connectedPumps != null && _connectedPumps.Count > 0)
        {
            Gizmos.color = Color.red;

            // Iterate through each GameObject in the list.
            foreach (WaterSourcePump pump in _connectedPumps)
            {
                // Make sure the target GameObject is not null.
                if (pump.gameObject != null)
                {
                    // Draw the line from the current GameObject's position to the target's position.
                    Gizmos.DrawLine(transform.position, pump.gameObject.transform.position);
                }
            }
        }

        //Draw line to parent water input
        if (_parentWaterInput != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, _parentWaterInput.gameObject.transform.position);
        }
    }
}
