using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private WaterInput _connectedWaterInput;

    [SerializeField] private List<Sprite> _emptySprites;
    [SerializeField] private List<Sprite> _fullSprites;

    [Header("References")]
    private SpriteRenderer _renderer;

    [Header("Data")]
    private Sprite _emptySprite;
    private Sprite _fullSprite;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();

        _emptySprite = _emptySprites[Random.Range(0, _emptySprites.Count)];
        _fullSprite = _fullSprites[Random.Range(0, _fullSprites.Count)];

        _renderer.sprite = _emptySprite;
    }

    private void FixedUpdate()
    {
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
}
