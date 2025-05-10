using RenderHeads.Services;
using System.Collections.Generic;
using UnityEngine;

public class WorldWaterManager : MonoService
{
    [Header("Data")]
    private List<WaterSource> _waterSources = new();
    private List<WaterPipe> _waterPipes = new();
    private List<WaterInput> _waterInputs = new();

    private List<WaterPipe> _waterPipesActive = new();

    private List<PowerSource> _powerSources = new();
    private List<WaterSourcePump> _waterSourcesPumps = new();
    private List<Cable> _cables = new();

    private List<Cable> _cablesActive = new();

    private void FixedUpdate()
    {
        CalculateWaterFlow();
        CalculateElectricityConnections();
    }

    public void AddWaterSource(WaterSource waterSource)
    {
        _waterSources.Add(waterSource);
    }

    public void AddPowerSource(PowerSource powerSource)
    {
        _powerSources.Add(powerSource);
    }

    public void AddWaterPump(WaterSourcePump waterSourcePump)
    {
        _waterSourcesPumps.Add(waterSourcePump);
    }

    public void AddWaterPipe(WaterPipe waterPipe)
    {
        _waterPipes.Add(waterPipe);
    }

    public void RemoveWaterPipe(WaterPipe waterPipe)
    {
        _waterPipes.Remove(waterPipe);
    }

    public void AddCable(Cable cable)
    {
        _cables.Add(cable);
    }

    public void RemoveCable(Cable cable)
    {
        _cables.Remove(cable);
    }

    public void AddWaterInput(WaterInput waterInput)
    {
        _waterInputs.Add(waterInput);
    }

    private void CalculateWaterFlow()
    {
        _waterPipesActive.Clear();

        foreach (WaterInput waterInput in _waterInputs)
        {
            waterInput.ClearConnectedWaterSources();
        }

        foreach (WaterSource waterSource in _waterSources)
        {
            if (waterSource.GetConnectedPipes().Count == 0 || !waterSource.GetIsPowered() || waterSource.CheckParentWaterInput())
            {
                continue;
            }

            Queue<WaterPipe> queue = new();
            HashSet<WaterPipe> visited = new();

            foreach (WaterPipe pipe in waterSource.GetConnectedPipes())
            {
                queue.Enqueue(pipe);
                visited.Add(pipe);
                _waterPipesActive.Add(pipe);
            }

            while (queue.Count > 0)
            {
                WaterPipe currentPipe = queue.Dequeue();

                _waterPipesActive.Add(currentPipe);

                if (currentPipe.isEndpoint == true)
                {
                    //Debug.Log("Found attached water input!");
                    foreach (WaterInput input in currentPipe.GetAttachedInputs())
                    {
                        input.AddConnectedWaterSource(waterSource);
                    }
                }

                foreach (WaterPipe neighbor in currentPipe.GetattachedPipes())
                {
                    if (neighbor != null && !visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        //Set the visuals
        foreach (WaterPipe waterPipe in _waterPipes)
        {
            if (_waterPipesActive.Contains(waterPipe))
            {
                waterPipe.hasWater = true;
            }
            else
            {
                waterPipe.hasWater = false;
            }
        }

    }

    private void CalculateElectricityConnections()
    {
        _cablesActive.Clear();

        foreach (WaterSourcePump pump in _waterSourcesPumps)
        {
            pump.ClearConnectedPowerSources();
        }

        foreach (PowerSource powerSource in _powerSources)
        {
            if (powerSource.GetConnectedCables().Count == 0 || !powerSource.GetIsActive())
            {
                continue;
            }

            Queue<Cable> queue = new();
            HashSet<Cable> visited = new();

            foreach (Cable cable in powerSource.GetConnectedCables())
            {
                queue.Enqueue(cable);
                visited.Add(cable);
                _cablesActive.Add(cable);
            }

            while (queue.Count > 0)
            {
                Cable currentCable = queue.Dequeue();

                _cablesActive.Add(currentCable);

                if (currentCable.isEndpoint == true)
                {
                    //Debug.Log("Found attached pump!");
                    foreach (WaterSourcePump pump in currentCable.GetAttachedPumps())
                    {
                        pump.AddConnectedPowerSource(powerSource);
                    }
                }

                foreach (Cable neighbor in currentCable.GetattachedCables())
                {
                    if (neighbor != null && !visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }

            //Set the visuals
            foreach (Cable cable in _cables)
            {
                if (_cablesActive.Contains(cable))
                {
                    cable.hasPower = true;
                }
                else
                {
                    cable.hasPower = false;
                }
            }
        }
    }
}
