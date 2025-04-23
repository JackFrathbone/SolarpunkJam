using RenderHeads.Services;
using System.Collections.Generic;
using UnityEngine;

public class WaterSource : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<PowerSource> _connectedPowerSources = new();

    [Header("Data")]
    private bool _isPowered;

    private List<WaterPipe> _connectedPipes = new();
    private List<WaterInput> _connectedWaterInputs = new();

    private RaycastHit2D _hit;

    //For keeping track of directions to raycast
    private readonly Vector2 _isometricUpRight = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 30f), Mathf.Sin(Mathf.Deg2Rad * 30f)).normalized;
    private readonly Vector2 _isometricDownLeft = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 210f), Mathf.Sin(Mathf.Deg2Rad * 210f)).normalized;
    private readonly Vector2 _isometricUpLeft = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 150f), Mathf.Sin(Mathf.Deg2Rad * 150f)).normalized;
    private readonly Vector2 _isometricDownRight = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 330f), Mathf.Sin(Mathf.Deg2Rad * 330f)).normalized;

    private LazyService<WorldWaterManager> _worldWaterManager;

    private void Start()
    {
        _worldWaterManager.Value.AddWaterSource(this);

        InvokeRepeating("CheckPowered", Random.Range(0f, 1f), 1f);
    }

    private void FixedUpdate()
    {
        if (!_isPowered)
        {
            return;
        }

        CheckForConnections();
    }

    private void CheckForConnections()
    {
        _connectedPipes.Clear();

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

        if (_hit.collider.CompareTag("Pipe"))
        {
            _connectedPipes.Add(_hit.collider.GetComponent<WaterPipe>());
        }
    }

    public List<WaterPipe> GetConnectedPipes()
    {
        return _connectedPipes;
    }

    public List<WaterInput> GetConnectedWaterInputs()
    {
        return _connectedWaterInputs;
    }

    public bool GetIsPowered()
    {
        return _isPowered;
    }

    private void CheckPowered()
    {
        if (_connectedPowerSources.Count == 0)
        {
            _isPowered = true;
            return;
        }

        //Go through all connected power sources, if one isnt powered up then turn off this water source
        bool poweredUp = true;
        foreach (PowerSource powerSource in _connectedPowerSources)
        {
            if (!powerSource.GetIsActive())
            {
                poweredUp = false;
            }
        }

        _isPowered = poweredUp;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        //Draw rays for each direction of detection
        Gizmos.DrawRay(transform.position, _isometricUpRight);
        Gizmos.DrawRay(transform.position, _isometricDownLeft);
        Gizmos.DrawRay(transform.position, _isometricUpLeft);
        Gizmos.DrawRay(transform.position, _isometricDownRight);

        //Draws lines to each connected powersource
        if (_connectedPowerSources != null && _connectedPowerSources.Count > 0)
        {
            Gizmos.color = Color.red;

            // Iterate through each GameObject in the list.
            foreach (PowerSource powerSource in _connectedPowerSources)
            {
                // Make sure the target GameObject is not null.
                if (powerSource.gameObject != null)
                {
                    // Draw the line from the current GameObject's position to the target's position.
                    Gizmos.DrawLine(transform.position, powerSource.gameObject.transform.position);
                }
            }
        }
    }
}
