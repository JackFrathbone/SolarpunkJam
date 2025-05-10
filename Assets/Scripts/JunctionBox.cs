using UnityEngine;

public class JunctionBox : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _flip;
    [SerializeField] LayerMask _layerMask;

    [Header("References")]
    private WaterPipe _connectedWaterPipeUpper;
    private WaterPipe _connectedWaterPipeLower;

    private Cable _connectedCableUpper;
    private Cable _connectedCableLower;

    private SpriteRenderer _renderer;

    [Header("Data")]
    private RaycastHit2D _hit;

    //For keeping track of directions to raycast
    private readonly Vector2 _isometricUpRight = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 30f), Mathf.Sin(Mathf.Deg2Rad * 30f)).normalized;
    private readonly Vector2 _isometricDownLeft = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 210f), Mathf.Sin(Mathf.Deg2Rad * 210f)).normalized;
    private readonly Vector2 _isometricUpLeft = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 150f), Mathf.Sin(Mathf.Deg2Rad * 150f)).normalized;
    private readonly Vector2 _isometricDownRight = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 330f), Mathf.Sin(Mathf.Deg2Rad * 330f)).normalized;

    private void OnValidate()
    {
        if (_flip)
        {
            _renderer = GetComponent<SpriteRenderer>();
            _renderer.flipX = true;
        }
        else
        {
            _renderer = GetComponent<SpriteRenderer>();
            _renderer.flipX = false;
        }
    }

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();

        if (_flip)
        {
            _renderer.flipX = true;
        }
        else
        {
            _renderer.flipX = false;
        }
    }

    private void FixedUpdate()
    {
        CheckForConnections();
        //BridgeConnections();
    }

    private void BridgeConnections()
    {
        if (_connectedWaterPipeUpper != null && _connectedWaterPipeLower != null)
        {
            _connectedWaterPipeUpper.AddPipe(_connectedWaterPipeLower);
            _connectedWaterPipeLower.AddPipe(_connectedWaterPipeUpper);
        }

        if (_connectedCableUpper != null && _connectedCableLower != null)
        {
            _connectedCableUpper.AddCable(_connectedCableLower);
            _connectedCableLower.AddCable(_connectedCableUpper);
        }
    }

    private void CheckForConnections()
    {
        _connectedWaterPipeUpper = null;
        _connectedWaterPipeLower = null;
        _connectedCableUpper = null;
        _connectedCableLower = null;

        gameObject.layer = 2;

        if (!_flip)
        {
            _hit = Physics2D.Raycast(transform.position, _isometricUpRight, 0.5f, _layerMask);
            CheckHitWaterPipe(0);

            _hit = Physics2D.Raycast(transform.position, _isometricDownLeft, 0.5f, _layerMask);
            CheckHitWaterPipe(1);

            _hit = Physics2D.Raycast(transform.position, _isometricUpLeft, 0.5f, _layerMask);
            CheckHitCable(2);

            _hit = Physics2D.Raycast(transform.position, _isometricDownRight, 0.5f, _layerMask);
            CheckHitCable(3);
        }
        else
        {
            _hit = Physics2D.Raycast(transform.position, _isometricUpRight, 0.5f, _layerMask);
            CheckHitCable(0);

            _hit = Physics2D.Raycast(transform.position, _isometricDownLeft, 0.5f, _layerMask);
            CheckHitCable(1);

            _hit = Physics2D.Raycast(transform.position, _isometricUpLeft, 0.5f, _layerMask);
            CheckHitWaterPipe(2);

            _hit = Physics2D.Raycast(transform.position, _isometricDownRight, 0.5f, _layerMask);
            CheckHitWaterPipe(3);
        }

        gameObject.layer = 0;
    }

    private void CheckHitWaterPipe(int direction)
    {
        if (_hit.collider == null)
        {
            return;
        }

        if (_hit.collider.CompareTag("Pipe"))
        {
            //If it is upper left or upper right
            if (direction == 0 || direction == 2)
            {
                _connectedWaterPipeUpper = _hit.collider.GetComponent<WaterPipe>();
            }
            else
            {
                _connectedWaterPipeLower = _hit.collider.GetComponent<WaterPipe>();
            }
        }
    }

    private void CheckHitCable(int direction)
    {
        if (_hit.collider == null)
        {
            return;
        }

        if (_hit.collider.CompareTag("Cable"))
        {
            //If it is upper left or upper right
            if (direction == 0 || direction == 2)
            {
                _connectedCableUpper = _hit.collider.GetComponent<Cable>();
            }
            else
            {
                _connectedCableLower = _hit.collider.GetComponent<Cable>();
            }
        }
    }

    public WaterPipe GetConnectedWaterPipe(WaterPipe pipe)
    {
        if (_connectedWaterPipeUpper != pipe)
        {
            return _connectedWaterPipeUpper;
        }
        else if (_connectedWaterPipeLower != pipe)
        {
            return _connectedWaterPipeLower;
        }
        else
        {
            return null;
        }
    }

    public Cable GetConnectedCable(Cable cable)
    {
        if (_connectedCableUpper != cable)
        {
            return _connectedCableUpper;
        }
        else if (_connectedCableLower != cable)
        {
            return _connectedCableLower;
        }
        else
        {
            return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!_flip)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, _isometricUpRight);
            Gizmos.DrawRay(transform.position, _isometricDownLeft);
            Gizmos.color = Color.black;
            Gizmos.DrawRay(transform.position, _isometricUpLeft);
            Gizmos.DrawRay(transform.position, _isometricDownRight);
        }
        else
        {
            Gizmos.color = Color.black;
            Gizmos.DrawRay(transform.position, _isometricUpRight);
            Gizmos.DrawRay(transform.position, _isometricDownLeft);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, _isometricUpLeft);
            Gizmos.DrawRay(transform.position, _isometricDownRight);
        }
    }
}
