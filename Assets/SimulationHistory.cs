using System;
using System.Collections.Generic;

public class SimulationHistory
{
    public Simulation CurrentTimeSim;
    public List<ResourceCollection> ResourcesAtEachTime;

    public SimulationHistory(Simulation currentTimeSim)
    {
        CurrentTimeSim = currentTimeSim;
        ResourcesAtEachTime = new List<ResourceCollection>();
        ResourcesAtEachTime.Add(new ResourceCollection(CurrentTimeSim.resources));
    }

    public void SetResourcesAtTime(int currentTime, ResourceCollection resources)
    {
        if (ResourcesAtEachTime.Count > currentTime)
        {
            ResourcesAtEachTime[currentTime] = new ResourceCollection(resources);
        } else if (ResourcesAtEachTime.Count == currentTime)
        {
            ResourcesAtEachTime.Add(new ResourceCollection(resources));
        } else
        {
            throw new ArgumentException($"{currentTime} is more than one step in the future");
        }
    }
}