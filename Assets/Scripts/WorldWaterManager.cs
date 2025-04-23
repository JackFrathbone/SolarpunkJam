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

    private void FixedUpdate()
    {
        FindWaterConnections();
    }

    public void AddWaterSource(WaterSource waterSource)
    {
        _waterSources.Add(waterSource);
    }

    public void AddWaterPipe(WaterPipe waterPipe)
    {
        _waterPipes.Add(waterPipe);
    }

    public void RemoveWaterPipe(WaterPipe waterPipe)
    {
        _waterPipes.Remove(waterPipe);
    }

    public void AddWaterInput(WaterInput waterInput)
    {
        _waterInputs.Add(waterInput);
    }

    private void FindWaterConnections()
    {
        _waterPipesActive.Clear();

        foreach (WaterInput waterInput in _waterInputs)
        {
            waterInput.ClearConnectedWaterSources();
        }

        foreach (WaterSource waterSource in _waterSources)
        {
            if (waterSource.GetConnectedPipes().Count == 0)
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
                    Debug.Log("Found attached water input!");
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
}
