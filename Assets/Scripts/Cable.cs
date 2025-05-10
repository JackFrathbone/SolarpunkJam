using RenderHeads.Services;
using System.Collections.Generic;
using UnityEngine;

public class Cable : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] LayerMask _layerMask;

    [SerializeField] AudioClip _placeAudioClip;

    [Header("References")]
    [SerializeField] List<Sprite> _pipeSprites = new();
    private SpriteRenderer _renderer;

    private ParticleSystem _particleSystem;

    private LazyService<GameManager> _gameManager;
    private LazyService<WorldWaterManager> _worldWaterManager;

    [Header("Data")]
    public bool isEndpoint;
    public bool hasPower;

    private RaycastHit2D _hit;

    private List<WaterSourcePump> _connectedPumps = new();
    private List<Cable> _connectedCables = new();
    private List<Cable> _newConnectedCables = new();

    //For keeping track of directions to raycast
    private readonly Vector2 _isometricUpRight = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 30f), Mathf.Sin(Mathf.Deg2Rad * 30f)).normalized;
    private readonly Vector2 _isometricDownLeft = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 210f), Mathf.Sin(Mathf.Deg2Rad * 210f)).normalized;
    private readonly Vector2 _isometricUpLeft = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 150f), Mathf.Sin(Mathf.Deg2Rad * 150f)).normalized;
    private readonly Vector2 _isometricDownRight = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 330f), Mathf.Sin(Mathf.Deg2Rad * 330f)).normalized;

    //Pipe direction bools
    private bool _upLeft;
    private bool _upright;
    private bool _downleft;
    private bool _downRight;

    private int _defaultLayer;
    private void Start()
    {
        _worldWaterManager.Value.AddCable(this);

        _renderer = GetComponentInChildren<SpriteRenderer>();
        _renderer.sprite = _pipeSprites[10];

        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _particleSystem.gameObject.SetActive(false);

        _defaultLayer = gameObject.layer;

        _gameManager.Value.PlayAudioClip(_placeAudioClip, 0.25f, Random.Range(1f, 1.5f));
    }

    private void FixedUpdate()
    {
        CheckForConnections();
        UpdateVisuals();
    }

    private void OnDestroy()
    {
        if (_gameManager.Value != null)
        {
            _gameManager.Value.PlayAudioClip(_placeAudioClip, 0.25f, 0.5f);
        }

        _worldWaterManager.Value.RemoveCable(this);
    }

    private void CheckForConnections()
    {
        _connectedPumps.Clear();
        _newConnectedCables.Clear();

        gameObject.layer = 2;

        _hit = Physics2D.Raycast(transform.position, _isometricUpRight, 0.5f, _layerMask);
        CheckHit(0);

        _hit = Physics2D.Raycast(transform.position, _isometricDownLeft, 0.5f, _layerMask);
        CheckHit(1);

        _hit = Physics2D.Raycast(transform.position, _isometricUpLeft, 0.5f, _layerMask);
        CheckHit(2);

        _hit = Physics2D.Raycast(transform.position, _isometricDownRight, 0.5f, _layerMask);
        CheckHit(3);

        gameObject.layer = _defaultLayer;

        // Discard old pipes that aren't connected anymore
        for (int i = _connectedCables.Count - 1; i >= 0; i--)
        {
            Cable cable = _connectedCables[i];
            if (!_newConnectedCables.Contains(cable))
            {
                _connectedCables.RemoveAt(i); // Remove by index
            }
        }
    }

    private void CheckHit(int direction)
    {
        if (_hit.collider == null)
        {
            switch (direction)
            {
                case 0:
                    _upright = false;
                    break;
                case 1:
                    _downleft = false;
                    break;
                case 2:
                    _upLeft = false;
                    break;
                case 3:
                    _downRight = false;
                    break;
            }

            return;
        }

        bool connected = false;
        if (_hit.collider.CompareTag("Cable"))
        {
            Cable cable = _hit.collider.GetComponent<Cable>();

            _newConnectedCables.Add(cable);

            if (!_connectedCables.Contains(cable))
            {
                _connectedCables.Add(cable);
            }

            connected = true;
        }
        else if (_hit.collider.CompareTag("WaterSourcePump"))
        {
            _connectedPumps.Add(_hit.collider.GetComponent<WaterSourcePump>());
            connected = true;
        }
        else if (_hit.collider.CompareTag("JunctionBox"))
        {
            Cable cable = _hit.collider.GetComponent<JunctionBox>().GetConnectedCable(this);

            if (cable != null)
            {
                _newConnectedCables.Add(cable);

                if (!_connectedCables.Contains(cable))
                {
                    _connectedCables.Add(cable);
                }
            }

            connected = true;
        }
        else if (_hit.collider.CompareTag("PowerSource"))
        {
            connected = true;
        }

        if (connected)
        {
            switch (direction)
            {
                case 0:
                    _upright = true;
                    break;
                case 1:
                    _downleft = true;
                    break;
                case 2:
                    _upLeft = true;
                    break;
                case 3:
                    _downRight = true;
                    break;
            }
        }

        //Set to an endpoint if it has any attached inputs
        if (_connectedPumps.Count != 0)
        {
            isEndpoint = true;
        }
    }

    private void UpdateVisuals()
    {
        //No connections
        if (!_upLeft && !_upright && !_downleft && !_downRight)
        {
            _renderer.sprite = _pipeSprites[10];
        }
        //Up left and up right
        else if (_upLeft && _upright && !_downleft && !_downRight)
        {
            _renderer.sprite = _pipeSprites[2];
        }
        //Down left and down right
        else if (!_upLeft && !_upright && _downleft && _downRight)
        {
            _renderer.sprite = _pipeSprites[0];
        }
        //Up left and down left
        else if (_upLeft && !_upright && _downleft && !_downRight)
        {
            _renderer.sprite = _pipeSprites[1];
        }
        //Up right and down right
        else if (!_upLeft && _upright && !_downleft && _downRight)
        {
            _renderer.sprite = _pipeSprites[3];
        }
        //Up left and down right
        else if (_upLeft && !_upright && !_downleft && _downRight)
        {
            _renderer.sprite = _pipeSprites[10];
        }
        //Up right and down left
        else if (!_upLeft && _upright && _downleft && !_downRight)
        {
            _renderer.sprite = _pipeSprites[9];
        }
        //Up left, down left and down right
        else if (_upLeft && !_upright && _downleft && _downRight)
        {
            _renderer.sprite = _pipeSprites[4];
        }
        //down left, up right and down right
        else if (!_upLeft && _upright && _downleft && _downRight)
        {
            _renderer.sprite = _pipeSprites[5];
        }
        //Down left, down right and up right
        else if (!_upLeft && _upright && _downleft && _downRight)
        {
            _renderer.sprite = _pipeSprites[6];
        }
        //Down left, up left and up right
        else if (_upLeft && _upright && _downleft && !_downRight)
        {
            _renderer.sprite = _pipeSprites[6];
        }
        //Up left, down right and up right
        else if (_upLeft && _upright && !_downleft && _downRight)
        {
            _renderer.sprite = _pipeSprites[7];
        }
        //Just down left
        else if (!_upLeft && !_upright && _downleft && !_downRight)
        {
            _renderer.sprite = _pipeSprites[9];
        }
        //Just up right
        else if (!_upLeft && _upright && !_downleft && !_downRight)
        {
            _renderer.sprite = _pipeSprites[9];
        }
        //Just down right
        else if (!_upLeft && !_upright && !_downleft && _downRight)
        {
            _renderer.sprite = _pipeSprites[10];
        }
        //Just up left
        else if (_upLeft && !_upright && !_downleft && !_downRight)
        {
            _renderer.sprite = _pipeSprites[10];
        }
        //All connections
        else if (_upLeft && _upright && _downleft && _downRight)
        {
            _renderer.sprite = _pipeSprites[8];
        }


        if (hasPower)
        {
            _particleSystem.gameObject.SetActive(true);
        }
        else
        {
            _particleSystem.gameObject.SetActive(false);
        }
    }

    public void AddCable(Cable cable)
    {
        if (!_connectedCables.Contains(cable))
        {
            _connectedCables.Add(cable);
        }
    }

    public List<Cable> GetattachedCables()
    {
        return _connectedCables;
    }

    public List<WaterSourcePump> GetAttachedPumps()
    {
        return _connectedPumps;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, _isometricUpRight);
        Gizmos.DrawRay(transform.position, _isometricDownLeft);
        Gizmos.DrawRay(transform.position, _isometricUpLeft);
        Gizmos.DrawRay(transform.position, _isometricDownRight);
    }
}
