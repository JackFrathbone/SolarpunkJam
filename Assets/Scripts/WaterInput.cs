using System.Collections.Generic;
using UnityEngine;

public class WaterInput : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<Blocker> _connectedBlockers = new();

    [Header("Data")]
    private bool _hasWater;

    private List<WaterPipe> _connectedPipes = new();
    private RaycastHit2D _hit;

    //For keeping track of directions to raycast
    private readonly Vector2 _isometricUpRight = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 30f), Mathf.Sin(Mathf.Deg2Rad * 30f)).normalized;
    private readonly Vector2 _isometricDownLeft = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 210f), Mathf.Sin(Mathf.Deg2Rad * 210f)).normalized;
    private readonly Vector2 _isometricUpLeft = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 150f), Mathf.Sin(Mathf.Deg2Rad * 150f)).normalized;
    private readonly Vector2 _isometricDownRight = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 330f), Mathf.Sin(Mathf.Deg2Rad * 330f)).normalized;

    private void Start()
    {
        Deactivate();
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

        if (_connectedPipeHasWater != _hasWater)
        {
            UpdateState(_connectedPipeHasWater);
        }
    }

    private void UpdateState(bool hasWater)
    {
        _hasWater = hasWater;

        if (_hasWater)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    private void Activate()
    {
        foreach (Blocker blocker in _connectedBlockers)
        {
            blocker.OpenBlocker();
        }
    }

    private void Deactivate()
    {
        foreach (Blocker blocker in _connectedBlockers)
        {
            blocker.CloseBlocker();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, _isometricUpRight);
        Gizmos.DrawRay(transform.position, _isometricDownLeft);
        Gizmos.DrawRay(transform.position, _isometricUpLeft);
        Gizmos.DrawRay(transform.position, _isometricDownRight);
    }
}
