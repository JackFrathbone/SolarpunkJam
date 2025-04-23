using RenderHeads.Services;
using System.Collections.Generic;
using UnityEngine;

public class WaterPipe : MonoBehaviour
{
    [Header("References")]
    private SpriteRenderer _renderer;

    [Header("References")]
    private LazyService<WorldWaterManager> _worldWaterManager;

    [Header("Data")]
    public bool isEndpoint;
    public bool hasWater;

    private RaycastHit2D _hit;

    private List<WaterInput> _connectedWaterInputs = new();
    private List<WaterPipe> _connectedPipes = new();

    //For keeping track of directions to raycast
    private readonly Vector2 _isometricUpRight = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 30f), Mathf.Sin(Mathf.Deg2Rad * 30f)).normalized;
    private readonly Vector2 _isometricDownLeft = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 210f), Mathf.Sin(Mathf.Deg2Rad * 210f)).normalized;
    private readonly Vector2 _isometricUpLeft = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 150f), Mathf.Sin(Mathf.Deg2Rad * 150f)).normalized;
    private readonly Vector2 _isometricDownRight = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 330f), Mathf.Sin(Mathf.Deg2Rad * 330f)).normalized;

    private void Start()
    {
        _worldWaterManager.Value.AddWaterPipe(this);

        _renderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        CheckForConnections();
        UpdateVisuals();
    }

    private void OnDestroy()
    {
        _worldWaterManager.Value.RemoveWaterPipe(this);
    }

    private void CheckForConnections()
    {
        _connectedWaterInputs.Clear();
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
        else if (_hit.collider.CompareTag("WaterInput"))
        {
            _connectedWaterInputs.Add(_hit.collider.GetComponent<WaterInput>());
        }

        //Set to an endpoint if it has any attached inputs
        if (_connectedWaterInputs.Count != 0)
        {
            isEndpoint = true;
        }
    }

    private void UpdateVisuals()
    {
        if (hasWater)
        {
            _renderer.color = Color.blue;
        }
        else
        {
            _renderer.color = Color.black;
        }
    }

    public List<WaterPipe> GetattachedPipes()
    {
        return _connectedPipes;
    }

    public List<WaterInput> GetAttachedInputs()
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
