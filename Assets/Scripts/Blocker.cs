using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocker : MonoBehaviour
{
    [Header("References")]
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;

    [Header("Data")]
    private Color _startColour;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();

        _startColour = _spriteRenderer.color;
    }

    public void OpenBlocker()
    {
        _spriteRenderer.color = Color.clear;
        _collider.enabled = false;
    }

    public void CloseBlocker()
    {
        _spriteRenderer.color = _startColour;
        _collider.enabled = true;
    }
}
