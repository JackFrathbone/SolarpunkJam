using RenderHeads.Services;
using System.Collections.Generic;
using UnityEngine;

public class WaterSourcePump : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<PowerSource> _connectedPowerSources = new();

    [SerializeField] private GameObject _activeIcon;

    LazyService<WorldWaterManager> _worldWaterManager;

    private void Start()
    {
        _worldWaterManager.Value.AddWaterPump(this);

        _activeIcon.SetActive(false);
    }

    private void UpdateActiveIcon()
    {
        if (_connectedPowerSources.Count == 0)
        {
            _activeIcon.SetActive(false);
        }
        else
        {
            bool hasPower = false;
            foreach (PowerSource powerSource in _connectedPowerSources)
            {
                if (powerSource.GetIsActive())
                {
                    hasPower = true;
                }
            }

            _activeIcon.SetActive(hasPower);
        }
    }

    public List<PowerSource> GetConnectedPowerSources()
    {
        return _connectedPowerSources;
    }

    public void ClearConnectedPowerSources()
    {
        _connectedPowerSources.Clear();
        UpdateActiveIcon();
    }

    public void AddConnectedPowerSource(PowerSource powerSource)
    {
        if (!_connectedPowerSources.Contains(powerSource))
        {
            _connectedPowerSources.Add(powerSource);
        }

        UpdateActiveIcon();
    }

    public void RemoveConnectedPowerSource(PowerSource powerSource)
    {
        if (_connectedPowerSources.Contains(powerSource))
        {
            _connectedPowerSources.Remove(powerSource);
        }

        UpdateActiveIcon();
    }
}
