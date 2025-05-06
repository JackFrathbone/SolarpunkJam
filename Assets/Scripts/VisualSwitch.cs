using System.Collections.Generic;
using UnityEngine;

public class VisualSwitch : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<Sprite> _emptySprites = new();
    [SerializeField] private List<Sprite> _fullSprites= new();

    [SerializeField] private bool _startActive;

    [Header("References")]
    private WaterInput _connectedWaterInput;
    private SpriteRenderer _renderer;

    [Header("Data")]
    private Sprite _emptySprite;
    private Sprite _fullSprite;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();

        _emptySprite = _emptySprites[Random.Range(0, _emptySprites.Count)];
        _fullSprite = _fullSprites[Random.Range(0, _fullSprites.Count)];

        if (!_startActive)
        {
            _renderer.sprite = _emptySprite;
        }
        else
        {
            _renderer.sprite = _fullSprite;
        }
    }

    private void FixedUpdate()
    {
        if (_startActive)
        {
            return;
        }

        if (_connectedWaterInput != null)
        {
            if (_connectedWaterInput.GetHasWater())
            {
                _renderer.sprite = _fullSprite;
            }
            else
            {
                _renderer.sprite = _emptySprite;
            }
        }
    }

    public void AddWaterInput(WaterInput waterInput)
    {
        _connectedWaterInput = waterInput;
    }
}
