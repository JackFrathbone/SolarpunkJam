using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterInput : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<Blocker> _connectedBlockers = new();

    [Header("Data")]
    private bool _hasWater;

    private void Start()
    {
        Deactivate();
    }

    private void Activate()
    {
        foreach(Blocker blocker in _connectedBlockers)
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
}
