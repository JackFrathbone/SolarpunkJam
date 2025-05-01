using RenderHeads.Services;
using System.Collections.Generic;
using UnityEngine;

public class WaterSourcePump : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<PowerSource> _connectedPowerSources = new();

    LazyService<WorldWaterManager> _worldWaterManager;

    private void Start()
    {
        _worldWaterManager.Value.AddWaterPump(this);
    }

    public List<PowerSource> GetConnectedPowerSources()
    {
        return _connectedPowerSources;
    }

    public void ClearConnectedPowerSources()
    {
        _connectedPowerSources.Clear();
    }

    public void AddConnectedPowerSource(PowerSource powerSource)
    {
        if (!_connectedPowerSources.Contains(powerSource))
        {
            _connectedPowerSources.Add(powerSource);
        }
    }

    public void RemoveConnectedPowerSource(PowerSource powerSource)
    {
        if (_connectedPowerSources.Contains(powerSource))
        {
            _connectedPowerSources.Remove(powerSource);
        }
    }
}
