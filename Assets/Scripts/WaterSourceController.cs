using RenderHeads.Services;
using System.Collections.Generic;
using UnityEngine;

public class WaterSourceController : MonoBehaviour
{
    [Header("Data")]
    private List<WaterPipe> _connectedPipes = new();
    private List<WaterInput> _connectedWaterInputs = new();

    private List<WaterPipe> _checkedPipes = new();

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
    }

    private void FixedUpdate()
    {
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, _isometricUpRight);
        Gizmos.DrawRay(transform.position, _isometricDownLeft);
        Gizmos.DrawRay(transform.position, _isometricUpLeft);
        Gizmos.DrawRay(transform.position, _isometricDownRight);
    }
}
